# Protected Routes and Token Expiration Testing Guide

## Test 16.4: Protected Routes and Token Expiration

This guide provides detailed test cases for protected routes, authentication guards, and token expiration handling.

## Prerequisites

1. Backend API is running
2. Frontend is running
3. At least one test user exists
4. Understanding of browser localStorage/sessionStorage

## Test Cases

### Test Case 1: Redirect to Login When No Token

**Objective:** Verify unauthenticated users are redirected to login

**Steps:**
1. Open browser in incognito/private mode (or clear localStorage)
2. Verify no token exists:
   ```javascript
   localStorage.getItem('token') // Should be null
   ```
3. Try to navigate directly to protected routes:
   - `/dashboard`
   - `/projects`
   - `/employees`
   - `/organizations`
   - Any other protected route

**Expected Results:**
- ✓ User is immediately redirected to `/login`
- ✓ URL changes to `/login`
- ✓ Login page is displayed
- ✓ Optional: Message shown "กรุณาเข้าสู่ระบบเพื่อดำเนินการต่อ"
- ✓ After login, user may be redirected back to original destination

---

### Test Case 2: Access Protected Routes with Valid Token

**Objective:** Verify authenticated users can access protected routes

**Steps:**
1. Login successfully
2. Verify token exists in localStorage
3. Navigate to various protected routes:
   - `/dashboard`
   - `/projects`
   - `/employees`
   - `/organizations`

**Expected Results:**
- ✓ All protected routes are accessible
- ✓ No redirects to login page
- ✓ Content loads correctly
- ✓ User information is displayed (e.g., in navbar)

---

### Test Case 3: Token Expiration - Manual Test

**Objective:** Verify system handles expired tokens correctly

**Steps:**
1. Login successfully
2. Open browser DevTools Console
3. Manually expire the token:
   ```javascript
   // Get current token
   const token = localStorage.getItem('token');
   
   // Decode and modify expiration
   function modifyTokenExpiration(token) {
     const parts = token.split('.');
     const payload = JSON.parse(atob(parts[1]));
     
     // Set expiration to past
     payload.exp = Math.floor(Date.now() / 1000) - 3600; // 1 hour ago
     
     // Note: This won't work for real validation, 
     // but tests client-side expiration check
     const newPayload = btoa(JSON.stringify(payload));
     return parts[0] + '.' + newPayload + '.' + parts[2];
   }
   
   // For testing, just remove the token or wait for real expiration
   localStorage.removeItem('token');
   ```
4. Try to navigate to a protected route or make an API call

**Expected Results:**
- ✓ User is redirected to `/login`
- ✓ Message displayed: "เซสชันหมดอายุ กรุณาเข้าสู่ระบบใหม่"
- ✓ Token is removed from localStorage
- ✓ Auth store is cleared

---

### Test Case 4: Token Expiration - API Call

**Objective:** Verify 401 response handling from API

**Steps:**
1. Login successfully
2. Wait for token to expire (or manually expire on backend)
3. Try to perform an action that makes an API call
4. Backend should return 401 Unauthorized

**Expected Results:**
- ✓ Frontend intercepts 401 response
- ✓ User is redirected to `/login`
- ✓ Error message displayed
- ✓ Token is cleared from localStorage
- ✓ Auth store is reset

---

### Test Case 5: Logout Functionality

**Objective:** Verify logout clears session and redirects

**Steps:**
1. Login successfully
2. Navigate to dashboard or any protected route
3. Click logout button (usually in navbar/menu)

**Expected Results:**
- ✓ Token is removed from localStorage
- ✓ Auth store is cleared
- ✓ User is redirected to `/login`
- ✓ Cannot access protected routes without logging in again
- ✓ Optional: Success message "ออกจากระบบสำเร็จ"

---

### Test Case 6: Token Persistence Across Page Refresh

**Objective:** Verify token persists across page reloads

**Steps:**
1. Login successfully
2. Navigate to a protected route
3. Refresh the page (F5 or Ctrl+R)

**Expected Results:**
- ✓ User remains logged in
- ✓ Token still exists in localStorage
- ✓ Protected route still accessible
- ✓ User information still displayed
- ✓ No redirect to login

---

### Test Case 7: Token Persistence Across Browser Tabs

**Objective:** Verify authentication state across multiple tabs

**Steps:**
1. Login in Tab 1
2. Open Tab 2 and navigate to the application
3. Navigate to a protected route in Tab 2

**Expected Results:**
- ✓ User is logged in in Tab 2
- ✓ Same token is used
- ✓ Protected routes accessible in both tabs

**Additional Test:**
1. Logout in Tab 1
2. Try to access protected route in Tab 2

**Expected Results:**
- ✓ Tab 2 should also be logged out (if using shared storage events)
- ✓ Or Tab 2 will logout on next API call/navigation

---

### Test Case 8: Token Persistence Across Browser Close

**Objective:** Verify token behavior when browser is closed

**Steps:**
1. Login successfully
2. Close browser completely (not just tab)
3. Reopen browser and navigate to application

**Expected Results:**
- ✓ If "Remember Me" was checked: User still logged in
- ✓ If "Remember Me" was NOT checked: User logged out (depends on implementation)
- ✓ Token behavior matches expected persistence strategy

---

### Test Case 9: Invalid Token Handling

**Objective:** Verify system handles corrupted/invalid tokens

**Steps:**
1. Login successfully
2. Open DevTools Console
3. Corrupt the token:
   ```javascript
   localStorage.setItem('token', 'invalid.token.here');
   ```
4. Try to access a protected route or make an API call

**Expected Results:**
- ✓ System detects invalid token
- ✓ User is redirected to `/login`
- ✓ Error message displayed
- ✓ Invalid token is removed from localStorage

---

### Test Case 10: Concurrent Session Handling

**Objective:** Verify behavior with multiple active sessions

**Steps:**
1. Login on Browser 1
2. Login with same account on Browser 2
3. Perform actions in both browsers

**Expected Results:**
- ✓ Both sessions work independently (if allowed)
- ✓ Or: Second login invalidates first session (if single session enforced)
- ✓ Behavior matches security requirements

---

### Test Case 11: Navigation Guard - Before Each Route

**Objective:** Verify router guard checks authentication

**Steps:**
1. Logout or clear token
2. Try to navigate to protected routes using:
   - Direct URL entry
   - Browser back/forward buttons
   - Internal navigation links

**Expected Results:**
- ✓ All navigation attempts are intercepted
- ✓ User is redirected to `/login`
- ✓ Original destination may be saved for redirect after login

---

### Test Case 12: Public Routes Accessible Without Token

**Objective:** Verify public routes don't require authentication

**Steps:**
1. Logout or clear token
2. Navigate to public routes:
   - `/login`
   - `/register`
   - `/` (home page, if public)
   - `/about` (if exists)

**Expected Results:**
- ✓ All public routes are accessible
- ✓ No redirect to login
- ✓ Content loads correctly

---

### Test Case 13: Redirect After Login

**Objective:** Verify user is redirected to intended destination after login

**Steps:**
1. Logout
2. Try to access `/dashboard`
3. Get redirected to `/login`
4. Login successfully

**Expected Results:**
- ✓ After login, user is redirected to `/dashboard` (original destination)
- ✓ Not redirected to default home page
- ✓ URL matches intended destination

---

### Test Case 14: Token in API Requests

**Objective:** Verify token is sent in API request headers

**Steps:**
1. Login successfully
2. Open DevTools Network tab
3. Perform an action that makes an API call
4. Inspect the request headers

**Expected Results:**
- ✓ Request includes `Authorization` header
- ✓ Header value is `Bearer <token>`
- ✓ Token matches the one in localStorage
- ✓ All authenticated API calls include the header

---

### Test Case 15: Token Refresh (if implemented)

**Objective:** Verify token refresh mechanism

**Steps:**
1. Login successfully
2. Wait until token is close to expiration
3. Make an API call or navigate

**Expected Results:**
- ✓ System automatically refreshes token
- ✓ New token is stored in localStorage
- ✓ User remains logged in
- ✓ No interruption to user experience

---

### Test Case 16: Role-Based Access Control

**Objective:** Verify routes restricted by user role

**Steps:**
1. Login as regular user (non-admin)
2. Try to access admin-only routes:
   - `/admin`
   - `/users/manage`
   - `/settings/system`

**Expected Results:**
- ✓ Access is denied
- ✓ User is redirected or shown error message
- ✓ Message: "คุณไม่มีสิทธิ์เข้าถึงหน้านี้"

**Additional Test:**
1. Login as admin user
2. Access same routes

**Expected Results:**
- ✓ Admin can access all routes
- ✓ Content loads correctly

---

### Test Case 17: Session Timeout Warning (if implemented)

**Objective:** Verify user is warned before session expires

**Steps:**
1. Login successfully
2. Wait until close to token expiration (e.g., 5 minutes before)

**Expected Results:**
- ✓ Warning modal/notification appears
- ✓ Message: "เซสชันของคุณกำลังจะหมดอายุ"
- ✓ Option to extend session
- ✓ Countdown timer shown
- ✓ If extended: New token is issued
- ✓ If ignored: User is logged out when timer reaches zero

---

### Test Case 18: Back Button After Logout

**Objective:** Verify back button doesn't bypass authentication

**Steps:**
1. Login successfully
2. Navigate to several protected routes
3. Logout
4. Click browser back button

**Expected Results:**
- ✓ User is redirected to `/login`
- ✓ Protected content is not accessible
- ✓ Token is not restored

---

### Test Case 19: Deep Link Protection

**Objective:** Verify deep links to protected routes are secured

**Steps:**
1. Logout
2. Bookmark a protected route (e.g., `/projects/123`)
3. Close browser
4. Open bookmark

**Expected Results:**
- ✓ User is redirected to `/login`
- ✓ After login, user is redirected to `/projects/123`
- ✓ Deep link destination is preserved

---

### Test Case 20: API Interceptor Error Handling

**Objective:** Verify API interceptor handles various error scenarios

**Steps:**
1. Login successfully
2. Simulate various API errors:
   - 401 Unauthorized
   - 403 Forbidden
   - 500 Server Error
   - Network timeout

**Expected Results:**
- ✓ 401: Redirect to login, clear token
- ✓ 403: Show "Access Denied" message
- ✓ 500: Show "Server Error" message
- ✓ Timeout: Show "Connection Timeout" message
- ✓ User remains on current page (except for 401)

---

## Testing Utilities

### Check Token in Console
```javascript
// View token
const token = localStorage.getItem('token');
console.log('Token:', token);

// Decode token (client-side only, not verified)
function parseJwt(token) {
  try {
    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const jsonPayload = decodeURIComponent(
      atob(base64).split('').map(c => {
        return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
      }).join('')
    );
    return JSON.parse(jsonPayload);
  } catch (e) {
    return null;
  }
}

const payload = parseJwt(token);
console.log('Token Payload:', payload);
console.log('Expires:', new Date(payload.exp * 1000));
console.log('Is Expired:', Date.now() >= payload.exp * 1000);
```

### Clear Authentication
```javascript
// Clear token and reload
localStorage.removeItem('token');
sessionStorage.clear();
location.reload();
```

### Simulate Token Expiration
```javascript
// Remove token to simulate expiration
localStorage.removeItem('token');

// Or set a very old token (won't pass backend validation)
localStorage.setItem('token', 'expired.token.value');
```

### Monitor Storage Events
```javascript
// Listen for storage changes (useful for multi-tab testing)
window.addEventListener('storage', (e) => {
  console.log('Storage changed:', e.key, e.oldValue, e.newValue);
});
```

## Test Results Template

| Test Case | Status | Notes |
|-----------|--------|-------|
| 1. Redirect When No Token | ⬜ Pass / ⬜ Fail | |
| 2. Access with Valid Token | ⬜ Pass / ⬜ Fail | |
| 3. Token Expiration Manual | ⬜ Pass / ⬜ Fail | |
| 4. Token Expiration API | ⬜ Pass / ⬜ Fail | |
| 5. Logout Functionality | ⬜ Pass / ⬜ Fail | |
| 6. Persist Across Refresh | ⬜ Pass / ⬜ Fail | |
| 7. Persist Across Tabs | ⬜ Pass / ⬜ Fail | |
| 8. Persist Across Browser Close | ⬜ Pass / ⬜ Fail | |
| 9. Invalid Token Handling | ⬜ Pass / ⬜ Fail | |
| 10. Concurrent Sessions | ⬜ Pass / ⬜ Fail | |
| 11. Navigation Guard | ⬜ Pass / ⬜ Fail | |
| 12. Public Routes Access | ⬜ Pass / ⬜ Fail | |
| 13. Redirect After Login | ⬜ Pass / ⬜ Fail | |
| 14. Token in API Requests | ⬜ Pass / ⬜ Fail | |
| 15. Token Refresh | ⬜ Pass / ⬜ Fail / ⬜ N/A | |
| 16. Role-Based Access | ⬜ Pass / ⬜ Fail | |
| 17. Session Timeout Warning | ⬜ Pass / ⬜ Fail / ⬜ N/A | |
| 18. Back Button After Logout | ⬜ Pass / ⬜ Fail | |
| 19. Deep Link Protection | ⬜ Pass / ⬜ Fail | |
| 20. API Interceptor Errors | ⬜ Pass / ⬜ Fail | |

## Security Checklist

- [ ] Tokens are stored securely (localStorage/httpOnly cookies)
- [ ] Tokens are not exposed in URLs
- [ ] Tokens are not logged to console in production
- [ ] Expired tokens are rejected
- [ ] Invalid tokens are rejected
- [ ] Protected routes require valid authentication
- [ ] Role-based access control works correctly
- [ ] Logout clears all authentication data
- [ ] No authentication bypass vulnerabilities
- [ ] XSS protection in place
- [ ] CSRF protection in place (if using cookies)

**Tested By:** _______________  
**Date:** _______________  
**Browser:** _______________  
**OS:** _______________
