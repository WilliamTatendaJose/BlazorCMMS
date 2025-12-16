# Work Order Quick Reference Guide

## ?? Quick Start

### View Work Order Details
1. Navigate to **RBM** ? **Work Orders**
2. Click **"??? View"** button on any work order
3. View complete details, timeline, and perform actions

### Create Work Order
1. Click **"? Create Work Order"** button
2. Select asset from dropdown
3. Set priority, type, due date
4. Assign to technician
5. Add description
6. Click **"Create Work Order"**

### Status Workflow

```
???????????????
?  Requested  ? 
???????????????
       ?
       ?
???????????????????
? Pending Approval?
???????????????????
     ?        ?
Approve    Reject
     ?        ?
     ?        ?
?????????? ????????????
? Approved? ? Rejected ?
?????????? ????????????
     ?
     ?
??????????
?  Open  ?
??????????
     ? Start
     ?
???????????????     ???????????
? In Progress ??????? On Hold ?
???????????????????????????????
       ?              Resume
       ? Complete
       ?
?????????????
? Completed ?
?????????????
```

## ?? Action Buttons Reference

### By Status

| Status | Available Actions |
|--------|------------------|
| **Requested / Pending** | ? Approve, ? Reject |
| **Approved / Open** | ?? Start Work |
| **In Progress** | ?? Hold, ? Complete |
| **On Hold** | ?? Resume |
| **All** | ??? Print, ?? Edit |

## ?? Field Mapping

### Essential Fields
| Field | Required | Description |
|-------|----------|-------------|
| **Asset** | ? Yes | The equipment being maintained |
| **Priority** | ? Yes | Low, Medium, High, Critical |
| **Type** | ? Yes | Preventive, Corrective, Predictive |
| **Description** | ? Yes | What work needs to be done |
| **Due Date** | ? Yes | When work must be completed |
| **Assigned To** | ?? Recommended | Technician responsible |

### Engineering Job Card Fields
| Field | Purpose |
|-------|---------|
| **Job Type** | Mechanical, Electrical, or Both |
| **Lock Out Required** | Safety lockout needed |
| **Fault Date/Time** | When issue occurred |
| **Originator** | Who reported the issue |
| **Department** | Requesting department |
| **Artisan Name** | Who performed the work |

### Time Tracking
| Field | When Set | Auto-Calculated |
|-------|----------|-----------------|
| **Created Date** | On creation | Yes |
| **Requested Date** | On creation | Yes |
| **Approved Date** | On approval | Yes |
| **Started Date** | On start | Yes |
| **Completed Date** | On completion | Yes |
| **Time Submitted** | On start | Yes |
| **Time Done** | On completion | Yes |
| **Total Work Time** | N/A | Yes (Done - Submitted) |

### Cost Tracking
| Field | When | Comparison |
|-------|------|------------|
| **Estimated Downtime** | Creation | vs Actual Downtime |
| **Estimated Cost** | Creation | vs Actual Cost |
| **Actual Downtime** | Completion | Shows variance |
| **Actual Cost** | Completion | Shows variance |
| **Labor Hours** | Completion | Total labor |

## ?? Status Color Codes

| Status | Color | Badge |
|--------|-------|-------|
| Requested | Purple | ![#9c27b0](https://via.placeholder.com/15/9c27b0/9c27b0.png) |
| Pending Approval | Orange | ![#ff9800](https://via.placeholder.com/15/ff9800/ff9800.png) |
| Approved | Green | ![#4caf50](https://via.placeholder.com/15/4caf50/4caf50.png) |
| Rejected | Red | ![#f44336](https://via.placeholder.com/15/f44336/f44336.png) |
| Open | Blue | ![#2196f3](https://via.placeholder.com/15/2196f3/2196f3.png) |
| In Progress | Orange | ![#ff9800](https://via.placeholder.com/15/ff9800/ff9800.png) |
| On Hold | Gray | ![#9e9e9e](https://via.placeholder.com/15/9e9e9e/9e9e9e.png) |
| Completed | Green | ![#4caf50](https://via.placeholder.com/15/4caf50/4caf50.png) |
| Cancelled | Gray | ![#9e9e9e](https://via.placeholder.com/15/9e9e9e/9e9e9e.png) |

## ??? Priority Color Codes

| Priority | Color | Use When |
|----------|-------|----------|
| **Critical** | Red ![#f44336](https://via.placeholder.com/15/f44336/f44336.png) | Safety issue, production stopped |
| **High** | Orange ![#ff9800](https://via.placeholder.com/15/ff9800/ff9800.png) | Production impacted, degraded performance |
| **Medium** | Blue ![#2196f3](https://via.placeholder.com/15/2196f3/2196f3.png) | Scheduled maintenance, minor issues |
| **Low** | Green ![#4caf50](https://via.placeholder.com/15/4caf50/4caf50.png) | Cosmetic, non-urgent improvements |

## ?? Navigation Paths

### From Dashboard
```
RBM Dashboard ? Work Orders Card ? Click "View All" ? Work Orders List
```

### From Sidebar
```
RBM Menu ? Work Orders ? Work Orders List
```

### Direct URL
```
/rbm/work-orders              ? List view
/rbm/work-orders/{id}        ? Detail view
/rbm/work-orders/create      ? Create (future)
/rbm/work-orders/{id}/edit   ? Edit (future)
```

## ?? Permission Matrix

| Role | View | Create | Edit | Approve | Delete | Assign |
|------|------|--------|------|---------|--------|--------|
| **Admin** | ? | ? | ? | ? | ? | ? |
| **Reliability Engineer** | ? | ? | ? | ? | ? | ? |
| **Planner** | ? | ? | ? | ? | ? | ? |
| **Supervisor** | ? | ? | ? | ? | ? | ? |
| **Technician** | ? | ? | ?* | ? | ? | ? |
| **Guest** | ? | ? | ? | ? | ? | ? |

*Technicians can only edit work orders assigned to them

## ?? Service Methods Cheat Sheet

### Get Operations
```csharp
// Single work order
var wo = await WorkOrderService.GetWorkOrderAsync(id);

// All work orders
var all = await WorkOrderService.GetAllWorkOrdersAsync();

// By status
var open = await WorkOrderService.GetWorkOrdersByStatusAsync("Open");

// By asset
var assetWOs = await WorkOrderService.GetWorkOrdersByAssetAsync(assetId);

// Overdue
var overdue = await WorkOrderService.GetOverdueWorkOrdersAsync();

// Pending approval
var pending = await WorkOrderService.GetPendingApprovalAsync();
```

### Status Operations
```csharp
// Approve
await WorkOrderService.ApproveWorkOrderAsync(id, "Approved - urgent");

// Reject
await WorkOrderService.RejectWorkOrderAsync(id, "Insufficient details");

// Start
await WorkOrderService.StartWorkOrderAsync(id);

// Hold
await WorkOrderService.HoldWorkOrderAsync(id, "Waiting for parts");

// Resume
await WorkOrderService.ResumeWorkOrderAsync(id);

// Complete
var completionData = new WorkOrderCompletionData { ... };
await WorkOrderService.CompleteWorkOrderAsync(id, completionData);

// Cancel
await WorkOrderService.CancelWorkOrderAsync(id, "Duplicate work order");
```

### Other Operations
```csharp
// Create
var newWO = new WorkOrder { ... };
await WorkOrderService.CreateWorkOrderAsync(newWO);

// Assign
await WorkOrderService.AssignWorkOrderAsync(id, "John Doe");

// Add spare part
var spare = new WorkOrderSpareUsed { ... };
await WorkOrderService.AddSparePartUsageAsync(woId, spare);

// Get statistics
var stats = await WorkOrderService.GetStatisticsAsync();
```

## ?? Statistics Available

```csharp
var stats = await WorkOrderService.GetStatisticsAsync();

// Access:
stats.Total                      // Total work orders
stats.Open                       // Open work orders
stats.InProgress                 // In progress
stats.Completed                  // Completed
stats.Overdue                    // Overdue count
stats.PendingApproval           // Waiting for approval
stats.CompletedThisMonth        // Completed this month
stats.AverageCompletionTime     // Avg hours to complete
stats.TotalCostThisMonth        // Total cost MTD
```

## ?? Common Tasks

### 1. Approve Multiple Work Orders
```csharp
foreach (var woId in selectedWorkOrderIds)
{
    await WorkOrderService.ApproveWorkOrderAsync(woId, "Batch approved");
}
```

### 2. Find Overdue Critical Work Orders
```csharp
var overdue = await WorkOrderService.GetOverdueWorkOrdersAsync();
var critical = overdue.Where(wo => wo.Priority == "Critical").ToList();
```

### 3. Get Asset Work History
```csharp
var history = await WorkOrderService.GetWorkOrdersByAssetAsync(assetId);
var completed = history.Where(wo => wo.Status == "Completed")
                      .OrderByDescending(wo => wo.CompletedDate)
                      .ToList();
```

### 4. Calculate Asset Downtime
```csharp
var assetWOs = await WorkOrderService.GetWorkOrdersByAssetAsync(assetId);
var totalDowntime = assetWOs
    .Where(wo => wo.ActualDowntime.HasValue)
    .Sum(wo => wo.ActualDowntime.Value);
```

## ?? Keyboard Shortcuts (Future)

| Key | Action |
|-----|--------|
| `N` | New work order |
| `E` | Edit current |
| `P` | Print/Export |
| `S` | Save changes |
| `Esc` | Close modal/cancel |
| `?/?` | Navigate list |
| `Enter` | View selected |

## ?? Troubleshooting

### Work Order Not Saving
- ? Check required fields are filled
- ? Verify user has permission
- ? Check database connection
- ? Review browser console for errors

### Status Not Changing
- ? Verify user role permissions
- ? Check current status allows transition
- ? Ensure no validation errors
- ? Review service method return value

### Data Not Loading
- ? Check database connection
- ? Verify work order ID exists
- ? Review navigation parameters
- ? Check authentication state

### Timeline Not Displaying
- ? Ensure dates are set correctly
- ? Check status transitions are valid
- ? Verify CSS is loaded
- ? Review browser compatibility

## ?? Best Practices

### ? DO:
- Fill in all relevant fields
- Update status promptly
- Document completion thoroughly
- Track spare parts usage
- Record accurate times and costs
- Link to failure modes when applicable
- Use appropriate priority levels
- Assign to qualified technicians

### ? DON'T:
- Skip required fields
- Leave status outdated
- Forget to complete work orders
- Ignore overdue notifications
- Estimate costs without basis
- Create duplicate work orders
- Use wrong priority levels
- Assign without checking availability

## ?? Quick Help

| Issue | Solution |
|-------|----------|
| Can't create WO | Check permissions (Create Work Orders) |
| Can't approve | Need Approve permission |
| Can't see all WOs | Need View All permission |
| Timeline not showing | Ensure status was properly updated |
| Cost variance showing red | Actual > Estimated (review and document) |
| Parts not deducting | Verify spare part ID is set |

## ?? Related Pages

- [Work Order Model Documentation](./BlazorApp1/Models/WorkOrder.cs)
- [Service Layer Documentation](./WORK_ORDER_ENHANCEMENTS.md)
- [User Management Guide](./USER_MANAGEMENT_GUIDE.md)
- [Documents Guide](./DOCUMENTS_QUICK_START.md)
- [Spare Parts Guide](./SPARE_PARTS_FEATURE.md)

---

**Version**: 1.0  
**Last Updated**: December 2024  
**Quick Reference for**: RBM CMMS Work Order Module
