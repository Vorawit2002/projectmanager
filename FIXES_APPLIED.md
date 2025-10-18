# Vue Errors Fixed in CustomerAppointmentPlanListView

## Summary
Fixed multiple Vue warnings and errors in the CustomerAppointmentPlanListView and related components.

## Issues Fixed

### 1. Missing Emits Declaration in CardAppointmentDetailView
**Error:** `Extraneous non-emits event listeners (pagesize, pagenumber, refreshNeeded)`

**Fix:** Added emits declaration to `CardAppointmentDetailView.vue`
```typescript
emits: ['pagesize', 'pagenumber', 'refreshNeeded', 'update:highlightedId']
```

### 2. Invalid Prop Type for TextFieldDatepicker
**Error:** `Invalid prop: type check failed for prop "selectedDateTime". Expected String with value "...", got Date`

**Fix:** Updated `TextFieldDatepicker.vue` to accept both String and Date types
```typescript
selectedDateTime: {
  type: [String, Date] as any,
}
```

### 3. Date Objects Passed to TextFieldDatepicker in CreateCustomerDailySchedule
**Error:** Date objects were being passed directly instead of strings

**Fix:** Added computed properties to convert Date to ISO string format
```typescript
formattedStartDate(): string {
  return this.createCommand.startDate ? this.createCommand.startDate.toISOString() : ''
},
formattedEndDate(): string {
  return this.createCommand.endDate ? this.createCommand.endDate.toISOString() : ''
}
```

Updated template to use formatted dates:
```vue
:selectedDateTime="formattedStartDate"
:selectedDateTime="formattedEndDate"
```

### 4. Date Objects Passed to TextFieldDatepicker in UpdateCustomerDailySchedule
**Error:** Same as #3

**Fix:** Applied same solution - added computed properties and updated template bindings

### 5. Date Objects Passed to TextFieldDatepicker in CustomerAppointmentPlanListView
**Error:** Same as #3

**Fix:** Added computed properties with type checking:
```typescript
formattedRequestStartDate(): string {
  return this.request.startDate ? (typeof this.request.startDate === 'string' ? this.request.startDate : this.request.startDate.toISOString()) : ''
},
formattedRequestEndDate(): string {
  return this.request.endDate ? (typeof this.request.endDate === 'string' ? this.request.endDate : this.request.endDate.toISOString()) : ''
}
```

### 6. Missing Emits Declaration in CreateCustomerAppointmentPlan
**Error:** `Extraneous non-emits event listeners (created)`

**Fix:** Added 'created' to emits array
```typescript
emits: ['close', 'created']
```

### 7. Invalid Prop Type for ManageActivityDetail
**Error:** `Invalid prop: type check failed for prop "id". Expected String | Number, got Null`

**Fix:** Updated prop definition to allow null and made it optional
```typescript
id: {
  type: [String, Number, null],
  required: false,
  default: null,
}
```

## Files Modified
1. `src/client_web/src/components/CardAppointmentDetailView.vue`
2. `src/client_web/src/components/Datepicker/TextFieldDatepicker.vue`
3. `src/client_web/src/views/AppointmentPlan/CreateCustomerDailySchedule.vue`
4. `src/client_web/src/views/AppointmentPlan/UpdateCustomerDailySchedule.vue`
5. `src/client_web/src/views/AppointmentPlan/CustomerAppointmentPlanListView.vue`
6. `src/client_web/src/views/AppointmentPlan/CreateCustomerAppointmentPlan.vue`
7. `src/client_web/src/views/AppointmentPlan/ManageActivityDetail.vue`

## Result
All Vue warnings and type errors have been resolved. The application should now run without these console errors.
