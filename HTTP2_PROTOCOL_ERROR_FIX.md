# Fix for ERR_HTTP2_PROTOCOL_ERROR on GetActivityPlanWithPagination

## Problem
The API endpoint `/api/activityplan/GetActivityPlanWithPagination` was returning `net::ERR_HTTP2_PROTOCOL_ERROR 200 (OK)`, causing "Failed to fetch" errors in the frontend.

## Root Cause
HTTP/2 protocol errors with 200 OK status typically occur when:
1. Response headers exceed HTTP/2 limits
2. Response payload is too large or malformed
3. Multiple eager-loaded relationships create oversized responses

## Changes Made

### 1. Optimized Query Performance (`GetActivityPlanWithPaginationQuery.cs`)

**Removed unnecessary `.Include()` statements:**
- Removed explicit `.Include()` for `Employees`, `Projects`, `Organizations`, `EventTypes`
- Removed `.Include(ap => ap.ActivityPlanAttachments).ThenInclude(at => at.Attachments)` which was loading potentially large attachment data
- Removed `.Include(ap => ap.PlanNotes)`
- The `.Select()` projection already handles loading the necessary navigation properties

**Fixed summary count queries:**
- Changed from querying entire `_context.ActivityPlans` table
- Now applies the same filters (role-based, department, employee, date, event type) to summary counts
- This ensures counts are accurate and queries are more efficient

### 2. Added Kestrel HTTP/2 Configuration (`appsettings.json`)

```json
"Kestrel": {
  "Limits": {
    "MaxRequestBodySize": 104857600,
    "Http2": {
      "MaxStreamsPerConnection": 100,
      "HeaderTableSize": 65536,
      "MaxFrameSize": 32768,
      "MaxRequestHeaderFieldSize": 16384,
      "InitialConnectionWindowSize": 131072,
      "InitialStreamWindowSize": 98304
    }
  }
}
```

These settings:
- Increase HTTP/2 header table size to handle larger responses
- Optimize frame and window sizes for better throughput
- Prevent protocol errors from oversized headers

### 3. Added Response Compression (`DependencyInjection.cs` & `Program.cs`)

**Added service:**
```csharp
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});
```

**Added middleware:**
```csharp
app.UseResponseCompression();
```

This reduces payload size significantly, especially for JSON responses with repeated data structures.

## Testing
After applying these changes:
1. Restart the backend server
2. Clear browser cache
3. Test the `/api/activityplan/GetActivityPlanWithPagination` endpoint
4. Verify the response loads successfully without protocol errors

## Benefits
- Reduced response payload size
- Faster API response times
- More efficient database queries
- Accurate filtered summary counts
- Better HTTP/2 protocol compliance
- Improved overall application performance
