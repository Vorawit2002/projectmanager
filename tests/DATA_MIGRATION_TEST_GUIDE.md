# Data Migration Testing Guide

## Test 16.6: Data Migration

This guide provides detailed test cases for migrating existing users from the old OpenID Connect system to the new local authentication system.

## Prerequisites

1. Database backup created before migration
2. Existing users in AspNetUsers table
3. Administrator account with migration permissions
4. Backend API running
5. Understanding of database structure

## Migration Overview

The migration process should:
- Create default passwords for users without passwords
- Set `RequirePasswordChange` flag to true for migrated users
- Preserve existing user roles and permissions
- Maintain user IDs and relationships
- Not lose any user data

## Pre-Migration Checklist

- [ ] Database backup completed
- [ ] List of existing users documented
- [ ] User roles documented
- [ ] Test environment prepared
- [ ] Migration script reviewed
- [ ] Rollback plan prepared

## Test Cases

### Test Case 1: Pre-Migration Database State

**Objective:** Document current database state before migration

**Steps:**
1. Connect to database
2. Query existing users:
   ```sql
   SELECT 
     Id, 
     UserName, 
     Email, 
     PasswordHash,
     RequirePasswordChange,
     LastPasswordChangeDate
   FROM AspNetUsers;
   ```
3. Query user roles:
   ```sql
   SELECT 
     u.UserName,
     r.Name as RoleName
   FROM AspNetUsers u
   JOIN AspNetUserRoles ur ON u.Id = ur.UserId
   JOIN AspNetRoles r ON ur.RoleId = r.Id;
   ```
4. Document counts:
   ```sql
   SELECT COUNT(*) as TotalUsers FROM AspNetUsers;
   SELECT COUNT(*) as UsersWithPassword FROM AspNetUsers WHERE PasswordHash IS NOT NULL;
   SELECT COUNT(*) as UsersWithoutPassword FROM AspNetUsers WHERE PasswordHash IS NULL;
   ```

**Expected Results:**
- ✓ All queries execute successfully
- ✓ User counts documented
- ✓ Baseline data captured for comparison

---

### Test Case 2: Migration Endpoint Access Control

**Objective:** Verify only administrators can run migration

**Steps:**
1. Login as regular user (non-admin)
2. Try to call migration endpoint:
   ```bash
   curl -X POST http://localhost:5000/api/auth/migrate-users \
     -H "Authorization: Bearer USER_TOKEN"
   ```

**Expected Results:**
- ✓ Request is rejected
- ✓ 403 Forbidden response
- ✓ Error message: "Access Denied" or similar

**Additional Test:**
1. Login as administrator
2. Call migration endpoint with admin token

**Expected Results:**
- ✓ Request is accepted
- ✓ Migration proceeds

---

### Test Case 3: Execute Migration Script

**Objective:** Run the migration and verify it completes successfully

**Steps:**
1. Login as administrator
2. Call migration endpoint:
   ```bash
   curl -X POST http://localhost:5000/api/auth/migrate-users \
     -H "Authorization: Bearer ADMIN_TOKEN" \
     -H "Content-Type: application/json"
   ```
3. Monitor response

**Expected Results:**
- ✓ Migration completes successfully
- ✓ Response includes migration summary:
  ```json
  {
    "totalUsers": 50,
    "migratedUsers": 45,
    "skippedUsers": 5,
    "errors": []
  }
  ```
- ✓ No errors in response
- ✓ Reasonable execution time

---

### Test Case 4: Post-Migration Database State

**Objective:** Verify database changes after migration

**Steps:**
1. Query users after migration:
   ```sql
   SELECT 
     Id, 
     UserName, 
     Email, 
     PasswordHash,
     RequirePasswordChange,
     LastPasswordChangeDate
   FROM AspNetUsers;
   ```
2. Check password hashes:
   ```sql
   SELECT COUNT(*) as UsersWithPassword 
   FROM AspNetUsers 
   WHERE PasswordHash IS NOT NULL;
   ```
3. Check RequirePasswordChange flag:
   ```sql
   SELECT COUNT(*) as UsersRequiringPasswordChange 
   FROM AspNetUsers 
   WHERE RequirePasswordChange = 1;
   ```

**Expected Results:**
- ✓ All users have PasswordHash (not NULL)
- ✓ Users without previous passwords have RequirePasswordChange = true
- ✓ Users with previous passwords may have RequirePasswordChange = false
- ✓ No users lost during migration
- ✓ User count matches pre-migration count

---

### Test Case 5: Migrated User Can Login

**Objective:** Verify migrated users can login with default password

**Steps:**
1. Identify a user that was migrated (had no password before)
2. Try to login with default password:
   ```bash
   curl -X POST http://localhost:5000/api/auth/login \
     -H "Content-Type: application/json" \
     -d '{
       "emailOrUsername": "migrateduser@example.com",
       "password": "DefaultPassword123!"
     }'
   ```

**Expected Results:**
- ✓ Login successful
- ✓ JWT token returned
- ✓ Response includes user information
- ✓ RequirePasswordChange flag is true in response

---

### Test Case 6: RequirePasswordChange Flag

**Objective:** Verify RequirePasswordChange flag is set correctly

**Steps:**
1. Login with migrated user
2. Check response for RequirePasswordChange flag
3. Try to access protected resources

**Expected Results:**
- ✓ RequirePasswordChange is true for migrated users
- ✓ Application prompts user to change password
- ✓ User cannot proceed without changing password (if enforced)
- ✓ Or: User can proceed but sees warning/reminder

---

### Test Case 7: User Roles Preserved

**Objective:** Verify user roles are maintained after migration

**Steps:**
1. Query user roles after migration:
   ```sql
   SELECT 
     u.UserName,
     r.Name as RoleName
   FROM AspNetUsers u
   JOIN AspNetUserRoles ur ON u.Id = ur.UserId
   JOIN AspNetRoles r ON ur.RoleId = r.Id
   ORDER BY u.UserName;
   ```
2. Compare with pre-migration role data

**Expected Results:**
- ✓ All user roles are preserved
- ✓ No roles lost
- ✓ No roles added unexpectedly
- ✓ Role assignments match pre-migration state

---

### Test Case 8: User Permissions Preserved

**Objective:** Verify user permissions work after migration

**Steps:**
1. Login as migrated admin user
2. Try to access admin-only features
3. Login as migrated regular user
4. Try to access regular features

**Expected Results:**
- ✓ Admin users can access admin features
- ✓ Regular users can access regular features
- ✓ Permissions work as before migration
- ✓ No permission errors

---

### Test Case 9: User Relationships Preserved

**Objective:** Verify user relationships (e.g., created records) are maintained

**Steps:**
1. Query records created by users:
   ```sql
   SELECT 
     u.UserName,
     COUNT(p.Id) as ProjectCount
   FROM AspNetUsers u
   LEFT JOIN Projects p ON u.Id = p.CreatedBy
   GROUP BY u.UserName;
   ```
2. Verify user IDs haven't changed
3. Check foreign key relationships

**Expected Results:**
- ✓ User IDs are unchanged
- ✓ All relationships intact
- ✓ No orphaned records
- ✓ Created/Modified by fields still valid

---

### Test Case 10: Migration Idempotency

**Objective:** Verify migration can be run multiple times safely

**Steps:**
1. Run migration first time
2. Note results
3. Run migration again
4. Compare results

**Expected Results:**
- ✓ Second run doesn't duplicate data
- ✓ Second run skips already migrated users
- ✓ No errors on second run
- ✓ Database state remains consistent

---

### Test Case 11: Partial Migration Handling

**Objective:** Verify migration handles errors gracefully

**Steps:**
1. Simulate error condition (e.g., invalid user data)
2. Run migration
3. Check results

**Expected Results:**
- ✓ Migration continues despite individual errors
- ✓ Errors are logged
- ✓ Successful migrations are committed
- ✓ Failed users are reported
- ✓ Database remains in consistent state

---

### Test Case 12: Default Password Security

**Objective:** Verify default passwords are secure

**Steps:**
1. Check default password in migration script
2. Verify password meets requirements:
   - Minimum length (6+ characters)
   - Contains uppercase
   - Contains lowercase
   - Contains numbers
   - Contains special characters (if required)

**Expected Results:**
- ✓ Default password meets all requirements
- ✓ Default password is not easily guessable
- ✓ Default password is documented for administrators
- ✓ Users are forced to change it

---

### Test Case 13: Password Change After Migration

**Objective:** Verify migrated users can change their password

**Steps:**
1. Login with migrated user (default password)
2. Navigate to change password page
3. Change password to new password
4. Logout
5. Login with new password

**Expected Results:**
- ✓ Password change succeeds
- ✓ RequirePasswordChange flag is set to false
- ✓ LastPasswordChangeDate is updated
- ✓ Can login with new password
- ✓ Cannot login with old password

---

### Test Case 14: Migration Logging

**Objective:** Verify migration process is properly logged

**Steps:**
1. Run migration
2. Check application logs
3. Check database audit logs (if applicable)

**Expected Results:**
- ✓ Migration start is logged
- ✓ Each user migration is logged
- ✓ Errors are logged with details
- ✓ Migration completion is logged
- ✓ Summary statistics are logged

---

### Test Case 15: Rollback Procedure

**Objective:** Verify migration can be rolled back if needed

**Steps:**
1. Create database backup before migration
2. Run migration
3. Simulate need for rollback
4. Restore from backup
5. Verify system state

**Expected Results:**
- ✓ Backup restoration succeeds
- ✓ Database returns to pre-migration state
- ✓ Users can still login with old method (if applicable)
- ✓ No data loss

---

### Test Case 16: Migration Performance

**Objective:** Measure migration performance with large user base

**Steps:**
1. Note start time
2. Run migration
3. Note end time
4. Calculate duration

**Expected Results:**
- ✓ Migration completes in reasonable time
- ✓ Performance is acceptable for user count
- ✓ No timeout errors
- ✓ Database performance not degraded

**Benchmarks:**
| User Count | Expected Time |
|------------|---------------|
| 100 users | < 10 seconds |
| 1,000 users | < 1 minute |
| 10,000 users | < 10 minutes |

---

### Test Case 17: Email Notifications (if implemented)

**Objective:** Verify users are notified about migration

**Steps:**
1. Run migration
2. Check if emails are sent to users
3. Verify email content

**Expected Results:**
- ✓ Email sent to each migrated user
- ✓ Email contains default password (or instructions)
- ✓ Email explains password change requirement
- ✓ Email includes support contact information

---

### Test Case 18: Migration Documentation

**Objective:** Verify migration is properly documented

**Steps:**
1. Review migration documentation
2. Check if all steps are documented
3. Verify troubleshooting guide exists

**Expected Results:**
- ✓ Migration process is documented
- ✓ Default password is documented
- ✓ Rollback procedure is documented
- ✓ Common issues and solutions documented

---

### Test Case 19: User Data Integrity

**Objective:** Verify no user data is corrupted during migration

**Steps:**
1. Before migration, export user data:
   ```sql
   SELECT * FROM AspNetUsers;
   ```
2. Run migration
3. After migration, export user data again
4. Compare data (excluding password-related fields)

**Expected Results:**
- ✓ All non-password fields unchanged
- ✓ No data corruption
- ✓ Email addresses intact
- ✓ Usernames intact
- ✓ User IDs unchanged

---

### Test Case 20: Production Migration Plan

**Objective:** Verify production migration plan is complete

**Steps:**
1. Review production migration plan
2. Check all prerequisites
3. Verify communication plan
4. Confirm rollback plan

**Expected Results:**
- ✓ Migration scheduled during maintenance window
- ✓ Users notified in advance
- ✓ Backup plan in place
- ✓ Rollback plan tested
- ✓ Support team prepared
- ✓ Monitoring in place

---

## Migration SQL Queries

### Check Users Without Passwords
```sql
SELECT 
  Id, 
  UserName, 
  Email,
  PasswordHash
FROM AspNetUsers
WHERE PasswordHash IS NULL OR PasswordHash = '';
```

### Check RequirePasswordChange Status
```sql
SELECT 
  UserName,
  Email,
  RequirePasswordChange,
  LastPasswordChangeDate
FROM AspNetUsers
WHERE RequirePasswordChange = 1;
```

### Verify User Roles
```sql
SELECT 
  u.UserName,
  u.Email,
  STRING_AGG(r.Name, ', ') as Roles
FROM AspNetUsers u
LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
GROUP BY u.UserName, u.Email
ORDER BY u.UserName;
```

### Count Migration Statistics
```sql
-- Total users
SELECT COUNT(*) as TotalUsers FROM AspNetUsers;

-- Users with passwords
SELECT COUNT(*) as UsersWithPassword 
FROM AspNetUsers 
WHERE PasswordHash IS NOT NULL AND PasswordHash != '';

-- Users requiring password change
SELECT COUNT(*) as RequirePasswordChange 
FROM AspNetUsers 
WHERE RequirePasswordChange = 1;

-- Users by role
SELECT 
  r.Name as RoleName,
  COUNT(ur.UserId) as UserCount
FROM AspNetRoles r
LEFT JOIN AspNetUserRoles ur ON r.Id = ur.RoleId
GROUP BY r.Name;
```

## Test Results Template

| Test Case | Status | Notes |
|-----------|--------|-------|
| 1. Pre-Migration State | ⬜ Pass / ⬜ Fail | |
| 2. Access Control | ⬜ Pass / ⬜ Fail | |
| 3. Execute Migration | ⬜ Pass / ⬜ Fail | |
| 4. Post-Migration State | ⬜ Pass / ⬜ Fail | |
| 5. Migrated User Login | ⬜ Pass / ⬜ Fail | |
| 6. RequirePasswordChange | ⬜ Pass / ⬜ Fail | |
| 7. Roles Preserved | ⬜ Pass / ⬜ Fail | |
| 8. Permissions Preserved | ⬜ Pass / ⬜ Fail | |
| 9. Relationships Preserved | ⬜ Pass / ⬜ Fail | |
| 10. Idempotency | ⬜ Pass / ⬜ Fail | |
| 11. Error Handling | ⬜ Pass / ⬜ Fail | |
| 12. Password Security | ⬜ Pass / ⬜ Fail | |
| 13. Password Change | ⬜ Pass / ⬜ Fail | |
| 14. Logging | ⬜ Pass / ⬜ Fail | |
| 15. Rollback | ⬜ Pass / ⬜ Fail | |
| 16. Performance | ⬜ Pass / ⬜ Fail | |
| 17. Email Notifications | ⬜ Pass / ⬜ Fail / ⬜ N/A | |
| 18. Documentation | ⬜ Pass / ⬜ Fail | |
| 19. Data Integrity | ⬜ Pass / ⬜ Fail | |
| 20. Production Plan | ⬜ Pass / ⬜ Fail | |

## Migration Statistics

Document actual migration results:

| Metric | Count |
|--------|-------|
| Total Users Before | ___ |
| Total Users After | ___ |
| Users Migrated | ___ |
| Users Skipped | ___ |
| Errors Encountered | ___ |
| Migration Duration | ___ |
| Users Requiring Password Change | ___ |

## Common Issues and Solutions

### Issue: Migration fails with database error
**Solution:**
- Check database connection
- Verify user has sufficient permissions
- Check for database locks
- Review error logs

### Issue: Some users cannot login after migration
**Solution:**
- Verify default password is correct
- Check RequirePasswordChange flag
- Verify user account is not locked
- Check password hash was created

### Issue: User roles missing after migration
**Solution:**
- Check AspNetUserRoles table
- Verify foreign key relationships
- Review migration script for role handling
- Restore from backup if needed

### Issue: Migration takes too long
**Solution:**
- Run during off-peak hours
- Optimize migration script
- Consider batch processing
- Increase database resources

## Post-Migration Checklist

- [ ] All users migrated successfully
- [ ] User roles preserved
- [ ] User permissions working
- [ ] Migrated users can login
- [ ] Password change flow works
- [ ] No data loss
- [ ] Performance acceptable
- [ ] Logs reviewed
- [ ] Users notified
- [ ] Documentation updated
- [ ] Backup verified
- [ ] Monitoring in place

**Migration Performed By:** _______________  
**Date:** _______________  
**Environment:** _______________  
**Total Users Migrated:** _______________  
**Issues Encountered:** _______________  
**Resolution:** _______________
