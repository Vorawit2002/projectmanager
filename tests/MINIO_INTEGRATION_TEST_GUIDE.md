# MinIO Integration Testing Guide

## Test 16.5: MinIO Integration

This guide provides detailed test cases for MinIO Docker container integration, file upload/download functionality, and console access.

## Prerequisites

1. Docker and Docker Compose installed
2. Application running via docker-compose
3. MinIO container running
4. Backend configured to use MinIO
5. At least one test user account

## MinIO Configuration

### Expected Configuration (from docker-compose.yml)

```yaml
minio:
  image: minio/minio:latest
  ports:
    - "9000:9000"  # API
    - "9001:9001"  # Console
  environment:
    MINIO_ROOT_USER: minioadmin
    MINIO_ROOT_PASSWORD: minioadmin
  volumes:
    - minio_data:/data
```

### Application Configuration

```json
{
  "MinIO": {
    "Endpoint": "minio:9000",
    "AccessKey": "minioadmin",
    "SecretKey": "minioadmin",
    "UseSSL": false,
    "BucketName": "projectmanagement"
  }
}
```

## Test Cases

### Test Case 1: MinIO Container Startup

**Objective:** Verify MinIO container starts correctly with docker-compose

**Steps:**
1. Stop all containers:
   ```bash
   docker-compose down
   ```
2. Start containers:
   ```bash
   docker-compose up -d
   ```
3. Check MinIO container status:
   ```bash
   docker-compose ps minio
   ```
4. Check MinIO logs:
   ```bash
   docker-compose logs minio
   ```

**Expected Results:**
- ✓ MinIO container is running (status: Up)
- ✓ No error messages in logs
- ✓ Logs show: "API: http://minio:9000"
- ✓ Logs show: "Console: http://minio:9001"
- ✓ Health check passes (if configured)

---

### Test Case 2: MinIO Console Access

**Objective:** Verify MinIO web console is accessible

**Steps:**
1. Open browser
2. Navigate to `http://localhost:9001`
3. Login with credentials:
   - Username: `minioadmin`
   - Password: `minioadmin`

**Expected Results:**
- ✓ MinIO console login page loads
- ✓ Login successful with correct credentials
- ✓ MinIO dashboard is displayed
- ✓ Can see buckets list
- ✓ Can navigate through console UI

---

### Test Case 3: Bucket Creation

**Objective:** Verify application bucket exists or is created

**Steps:**
1. Login to MinIO console
2. Navigate to "Buckets" section
3. Look for `projectmanagement` bucket

**Expected Results:**
- ✓ `projectmanagement` bucket exists
- ✓ Bucket is accessible
- ✓ Bucket has correct permissions

**If bucket doesn't exist:**
1. Create bucket manually or via application
2. Verify bucket appears in console

---

### Test Case 4: File Upload via Application

**Objective:** Verify files can be uploaded through the application

**Steps:**
1. Login to application
2. Navigate to a feature that supports file upload (e.g., Attachments, Activity Plans)
3. Select a test file (e.g., test-image.jpg, test-document.pdf)
4. Upload the file
5. Verify upload success message

**Expected Results:**
- ✓ File upload completes successfully
- ✓ Success message displayed
- ✓ File appears in application UI
- ✓ No error messages

**Verification in MinIO Console:**
1. Login to MinIO console
2. Navigate to `projectmanagement` bucket
3. Browse to uploaded file location

**Expected Results:**
- ✓ File exists in MinIO
- ✓ File size matches original
- ✓ File name is correct (or follows naming convention)

---

### Test Case 5: File Download via Application

**Objective:** Verify files can be downloaded through the application

**Steps:**
1. Upload a file (from Test Case 4)
2. In application, click download button for the file
3. Check browser downloads folder

**Expected Results:**
- ✓ File downloads successfully
- ✓ Downloaded file size matches original
- ✓ Downloaded file can be opened
- ✓ File content is intact (not corrupted)

---

### Test Case 6: Multiple File Upload

**Objective:** Verify multiple files can be uploaded simultaneously

**Steps:**
1. Login to application
2. Navigate to file upload feature
3. Select multiple files (e.g., 3-5 files)
4. Upload all files at once

**Expected Results:**
- ✓ All files upload successfully
- ✓ Progress indicators show for each file
- ✓ All files appear in application
- ✓ All files exist in MinIO

---

### Test Case 7: Large File Upload

**Objective:** Verify large files can be uploaded

**Steps:**
1. Prepare a large test file (e.g., 50MB, 100MB)
2. Upload the file through application
3. Monitor upload progress

**Expected Results:**
- ✓ Large file uploads successfully
- ✓ Progress indicator shows upload progress
- ✓ No timeout errors
- ✓ File is complete in MinIO
- ✓ File can be downloaded successfully

---

### Test Case 8: File Type Validation

**Objective:** Verify file type restrictions work correctly

**Steps:**
1. Try to upload various file types:
   - Allowed: .jpg, .png, .pdf, .docx
   - Disallowed: .exe, .bat, .sh (if restricted)
2. Observe validation messages

**Expected Results:**
- ✓ Allowed file types upload successfully
- ✓ Disallowed file types are rejected
- ✓ Clear error message for rejected files
- ✓ Validation happens before upload (client-side)
- ✓ Backend also validates (server-side)

---

### Test Case 9: File Size Limit

**Objective:** Verify file size limits are enforced

**Steps:**
1. Check configured file size limit
2. Try to upload a file exceeding the limit
3. Try to upload a file within the limit

**Expected Results:**
- ✓ Files within limit upload successfully
- ✓ Files exceeding limit are rejected
- ✓ Clear error message: "ไฟล์มีขนาดใหญ่เกินกำหนด"
- ✓ Limit is enforced on both client and server

---

### Test Case 10: File Deletion

**Objective:** Verify files can be deleted

**Steps:**
1. Upload a test file
2. Delete the file through application
3. Verify deletion

**Expected Results:**
- ✓ File is removed from application UI
- ✓ File is deleted from MinIO
- ✓ Success message displayed
- ✓ File cannot be downloaded after deletion

**Verification in MinIO Console:**
- ✓ File no longer exists in bucket

---

### Test Case 11: MinIO Data Persistence

**Objective:** Verify data persists across container restarts

**Steps:**
1. Upload several test files
2. Note file names and locations
3. Stop MinIO container:
   ```bash
   docker-compose stop minio
   ```
4. Start MinIO container:
   ```bash
   docker-compose start minio
   ```
5. Check if files still exist

**Expected Results:**
- ✓ All files still exist after restart
- ✓ Files are accessible
- ✓ No data loss
- ✓ File metadata is preserved

---

### Test Case 12: MinIO Volume Persistence

**Objective:** Verify data persists across docker-compose down/up

**Steps:**
1. Upload test files
2. Stop all containers:
   ```bash
   docker-compose down
   ```
3. Start containers again:
   ```bash
   docker-compose up -d
   ```
4. Check if files still exist

**Expected Results:**
- ✓ All files still exist
- ✓ MinIO volume was preserved
- ✓ No data loss

---

### Test Case 13: MinIO API Connectivity

**Objective:** Verify application can connect to MinIO API

**Steps:**
1. Check application logs for MinIO connection
2. Upload a file
3. Check backend logs

**Expected Results:**
- ✓ No connection errors in logs
- ✓ Application successfully connects to MinIO
- ✓ API calls succeed
- ✓ Proper error handling if connection fails

---

### Test Case 14: MinIO Network Configuration

**Objective:** Verify MinIO is accessible within Docker network

**Steps:**
1. Check docker-compose network configuration
2. Verify application can reach MinIO at `minio:9000`
3. Test from application container:
   ```bash
   docker-compose exec web curl http://minio:9000/minio/health/live
   ```

**Expected Results:**
- ✓ MinIO is reachable from application container
- ✓ Health check returns success
- ✓ DNS resolution works (minio hostname)

---

### Test Case 15: File Access Permissions

**Objective:** Verify file access permissions are correct

**Steps:**
1. Upload a file as User A
2. Try to access/download file as User B
3. Check permission behavior

**Expected Results:**
- ✓ Files have appropriate access controls
- ✓ Users can only access their own files (if restricted)
- ✓ Or: All authenticated users can access files (if public)
- ✓ Behavior matches security requirements

---

### Test Case 16: MinIO Console File Management

**Objective:** Verify files can be managed through MinIO console

**Steps:**
1. Login to MinIO console
2. Navigate to `projectmanagement` bucket
3. Try to:
   - View file details
   - Download a file
   - Delete a file
   - Upload a file directly

**Expected Results:**
- ✓ All operations work in console
- ✓ Files uploaded via console appear in application
- ✓ Files deleted via console are removed from application

---

### Test Case 17: Error Handling - MinIO Unavailable

**Objective:** Verify application handles MinIO downtime gracefully

**Steps:**
1. Stop MinIO container:
   ```bash
   docker-compose stop minio
   ```
2. Try to upload a file through application
3. Try to download a file

**Expected Results:**
- ✓ Clear error message displayed
- ✓ Application doesn't crash
- ✓ Error: "ไม่สามารถเชื่อมต่อกับระบบจัดเก็บไฟล์ได้"
- ✓ User can retry after MinIO is back

---

### Test Case 18: Concurrent File Uploads

**Objective:** Verify multiple users can upload simultaneously

**Steps:**
1. Login as User A in Browser 1
2. Login as User B in Browser 2
3. Both users upload files at the same time

**Expected Results:**
- ✓ Both uploads succeed
- ✓ No conflicts or errors
- ✓ Each user's files are stored correctly
- ✓ No data corruption

---

### Test Case 19: File Metadata

**Objective:** Verify file metadata is stored correctly

**Steps:**
1. Upload a file
2. Check file metadata in application
3. Check file metadata in MinIO console

**Expected Results:**
- ✓ File name is correct
- ✓ File size is accurate
- ✓ Upload date/time is recorded
- ✓ Content type is correct
- ✓ Uploader information is stored (if applicable)

---

### Test Case 20: MinIO Bucket Policy

**Objective:** Verify bucket has correct access policy

**Steps:**
1. Login to MinIO console
2. Navigate to bucket settings
3. Check access policy

**Expected Results:**
- ✓ Bucket policy is configured
- ✓ Policy matches security requirements
- ✓ Public access is disabled (if required)
- ✓ Only application has access via credentials

---

## Testing Utilities

### Check MinIO Container Status
```bash
# Check if MinIO is running
docker-compose ps minio

# View MinIO logs
docker-compose logs -f minio

# Check MinIO health
curl http://localhost:9000/minio/health/live

# Access MinIO container shell
docker-compose exec minio sh
```

### MinIO CLI (mc) Commands
```bash
# Install MinIO client (if needed)
# macOS: brew install minio/stable/mc
# Linux: wget https://dl.min.io/client/mc/release/linux-amd64/mc

# Configure mc
mc alias set local http://localhost:9000 minioadmin minioadmin

# List buckets
mc ls local

# List files in bucket
mc ls local/projectmanagement

# Copy file to MinIO
mc cp test-file.txt local/projectmanagement/

# Download file from MinIO
mc cp local/projectmanagement/test-file.txt ./downloaded-file.txt

# Remove file
mc rm local/projectmanagement/test-file.txt
```

### Check MinIO from Application Container
```bash
# Access application container
docker-compose exec web bash

# Test MinIO connectivity
curl http://minio:9000/minio/health/live

# Check DNS resolution
nslookup minio
ping minio
```

### Backup MinIO Data
```bash
# Backup MinIO volume
docker run --rm -v projectmanagement_minio_data:/data -v $(pwd):/backup alpine tar czf /backup/minio-backup.tar.gz /data

# Restore MinIO volume
docker run --rm -v projectmanagement_minio_data:/data -v $(pwd):/backup alpine tar xzf /backup/minio-backup.tar.gz -C /
```

## Test Results Template

| Test Case | Status | Notes |
|-----------|--------|-------|
| 1. Container Startup | ⬜ Pass / ⬜ Fail | |
| 2. Console Access | ⬜ Pass / ⬜ Fail | |
| 3. Bucket Creation | ⬜ Pass / ⬜ Fail | |
| 4. File Upload | ⬜ Pass / ⬜ Fail | |
| 5. File Download | ⬜ Pass / ⬜ Fail | |
| 6. Multiple File Upload | ⬜ Pass / ⬜ Fail | |
| 7. Large File Upload | ⬜ Pass / ⬜ Fail | |
| 8. File Type Validation | ⬜ Pass / ⬜ Fail | |
| 9. File Size Limit | ⬜ Pass / ⬜ Fail | |
| 10. File Deletion | ⬜ Pass / ⬜ Fail | |
| 11. Data Persistence (Restart) | ⬜ Pass / ⬜ Fail | |
| 12. Volume Persistence (Down/Up) | ⬜ Pass / ⬜ Fail | |
| 13. API Connectivity | ⬜ Pass / ⬜ Fail | |
| 14. Network Configuration | ⬜ Pass / ⬜ Fail | |
| 15. File Access Permissions | ⬜ Pass / ⬜ Fail | |
| 16. Console File Management | ⬜ Pass / ⬜ Fail | |
| 17. Error Handling | ⬜ Pass / ⬜ Fail | |
| 18. Concurrent Uploads | ⬜ Pass / ⬜ Fail | |
| 19. File Metadata | ⬜ Pass / ⬜ Fail | |
| 20. Bucket Policy | ⬜ Pass / ⬜ Fail | |

## Common Issues and Solutions

### Issue: MinIO container won't start
**Solution:**
- Check if port 9000 or 9001 is already in use
- Check Docker logs: `docker-compose logs minio`
- Verify docker-compose.yml configuration

### Issue: Cannot access MinIO console
**Solution:**
- Verify MinIO container is running
- Check port mapping: `docker-compose ps`
- Try accessing via IP: `http://127.0.0.1:9001`
- Check firewall settings

### Issue: Application cannot connect to MinIO
**Solution:**
- Verify MinIO endpoint in configuration (should be `minio:9000` in Docker network)
- Check if containers are on same network
- Verify credentials match

### Issue: Files not persisting
**Solution:**
- Check if volume is defined in docker-compose.yml
- Verify volume exists: `docker volume ls`
- Check volume mount: `docker inspect <container_id>`

### Issue: Upload fails with timeout
**Solution:**
- Increase timeout settings
- Check file size limits
- Verify network connectivity
- Check MinIO container resources

## Performance Benchmarks

Document performance metrics:

| Operation | File Size | Time | Notes |
|-----------|-----------|------|-------|
| Upload | 1MB | ___ ms | |
| Upload | 10MB | ___ ms | |
| Upload | 50MB | ___ ms | |
| Download | 1MB | ___ ms | |
| Download | 10MB | ___ ms | |
| Download | 50MB | ___ ms | |
| Delete | Any | ___ ms | |

**Tested By:** _______________  
**Date:** _______________  
**Environment:** _______________
