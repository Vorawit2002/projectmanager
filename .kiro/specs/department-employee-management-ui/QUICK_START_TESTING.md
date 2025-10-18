# Quick Start Guide - Testing Department & Employee Management UI

## 🚀 Quick Start (5 Minutes)

### Step 1: Start the Application
```bash
# Terminal 1: Start Backend (if not already running)
cd ProjectManagement
dotnet run --project src/Web

# Terminal 2: Start Frontend
cd src/client_web
npm run dev
```

### Step 2: Open the Application
- Open browser: `http://localhost:5173`
- Login with test credentials

### Step 3: Navigate to Features
- **Departments**: `http://localhost:5173/MasterData/DepartmentListView`
- **Employees**: `http://localhost:5173/MasterData/EmployeeListView`

### Step 4: Quick Smoke Test (2 minutes)
1. ✅ Can you see the department list?
2. ✅ Can you create a new department?
3. ✅ Can you see the employee list?
4. ✅ Can you create a new employee?

If all 4 checks pass → System is working! Proceed to full testing.

---

## 📋 Full Testing Process

### Option A: Comprehensive Testing (2-3 hours)
Follow the detailed guide in `testing-guide.md`

### Option B: Critical Path Testing (30 minutes)
1. **Test Authentication** (5 min)
   - Login as Admin
   - Login as Manager
   - Try as Viewer (should be blocked)

2. **Test Department CRUD** (10 min)
   - Create a department
   - Edit the department
   - View department details
   - Delete the department

3. **Test Employee CRUD** (10 min)
   - Create an employee with image
   - Edit the employee
   - View employee details
   - Delete the employee

4. **Test Responsive Design** (5 min)
   - Resize browser to mobile size
   - Test drawer opens/closes
   - Verify table is usable

---

## 🎯 What to Test

### Must Test (Critical)
- [ ] Login with Admin role
- [ ] Create department
- [ ] Create employee
- [ ] Edit operations work
- [ ] Delete operations work
- [ ] Search functionality
- [ ] Pagination works

### Should Test (Important)
- [ ] Login with Manager role
- [ ] Login with Viewer role (should fail)
- [ ] Filter employees by department
- [ ] Filter employees by status
- [ ] Upload employee image
- [ ] View details (read-only)
- [ ] Responsive on mobile

### Nice to Test (Optional)
- [ ] Error handling (disconnect network)
- [ ] Validation messages
- [ ] ESC key closes drawers
- [ ] Browser back button
- [ ] Multiple browsers
- [ ] Performance with large datasets

---

## 📝 Recording Results

### Quick Method
Use `test-results.md` and mark each test:
- ✅ = Pass
- ❌ = Fail
- ⏳ = Pending
- 🚫 = Blocked

### Example
```markdown
### Test Case 1.1: Navigate to DepartmentListView
- **Status**: ✅ Pass
- **Actual Result**: Page loaded successfully, shows 10 departments
- **Notes**: All good!
```

---

## 🐛 Common Issues & Quick Fixes

### Issue: "Cannot access page"
**Fix**: Check if you're logged in with Admin or Manager role

### Issue: "No data showing"
**Fix**: Check if backend is running and database has data

### Issue: "Image upload fails"
**Fix**: Check file size (< 5MB) and type (jpg, png, gif)

### Issue: "Drawer doesn't open"
**Fix**: Check browser console for errors, refresh page

### Issue: "Permission denied"
**Fix**: Verify user has correct role (Admin or Manager)

---

## ✅ Quick Checklist

Before starting testing:
- [ ] Backend is running
- [ ] Frontend is running
- [ ] Test users are available (Admin, Manager, Viewer)
- [ ] Database has some test data
- [ ] Browser dev tools are open (F12)

During testing:
- [ ] Record all test results
- [ ] Take screenshots of issues
- [ ] Note any error messages
- [ ] Check browser console for errors

After testing:
- [ ] Fill in test-results.md summary
- [ ] Report critical bugs immediately
- [ ] Document any workarounds found
- [ ] Sign off on test-results.md

---

## 📞 Need Help?

### Documentation References
- **Detailed Testing**: See `testing-guide.md`
- **Test Cases**: See `test-results.md`
- **Implementation Details**: See `design.md`
- **Requirements**: See `requirements.md`

### Quick Checks
1. Is backend running? Check `http://localhost:5000/swagger`
2. Is frontend running? Check `http://localhost:5173`
3. Any console errors? Press F12 and check Console tab
4. Network issues? Check Network tab in dev tools

---

## 🎉 Success Criteria

Testing is complete when:
1. All critical tests pass ✅
2. No blocking bugs found 🐛
3. All user roles tested 👥
4. Responsive design verified 📱
5. Test results documented 📝

---

## 📊 Testing Progress Tracker

```
Total Test Cases: 35
├── Routes (5): ⏳ Pending
├── Department CRUD (7): ⏳ Pending
├── Employee CRUD (9): ⏳ Pending
├── Responsive (5): ⏳ Pending
├── Error Handling (4): ⏳ Pending
└── Permissions (4): ⏳ Pending

Status: ⏳ Not Started
```

Update this as you progress!

---

## 🚦 Test Status Indicators

- 🟢 **Green**: All tests passing, ready for production
- 🟡 **Yellow**: Minor issues found, can proceed with caution
- 🔴 **Red**: Critical issues found, needs fixes before deployment
- ⚪ **White**: Testing not started

**Current Status**: ⚪ Testing Not Started

---

**Ready to begin? Start with the Quick Smoke Test above! 🚀**

