# ?? SPARE PARTS - GETTING STARTED IN 5 MINUTES

## ? Start Here

This guide gets you using the Spare Parts feature in **5 minutes or less**.

---

## 1?? Access the Page (30 seconds)

### Option A: Direct URL
```
Navigate to: /rbm/spare-parts
```

### Option B: From Menu
```
1. Open RBM CMMS
2. Find "?? Spare Parts" in menu
3. Click to open
```

### What You See
- Page header with title
- 4 stat cards showing metrics
- Action buttons
- Filters
- Empty or populated table

---

## 2?? View Your Inventory (1 minute)

### See What You Have
```
1. Page loads with all parts
2. See parts in table
3. Check quantities
4. View prices
5. See locations
```

### Information Available
```
For Each Part:
? Part Number      (e.g., BRG-001)
? Name             (e.g., SKF Bearing)
? Quantity         (e.g., 25 units)
? Status           (In Stock / Low Stock)
? Unit Cost        (e.g., $45.00)
? Total Value      (e.g., $1,125.00)
? Location         (e.g., Shelf A-12)
```

---

## 3?? Find Low Stock Items (30 seconds)

### Quick Check
```
1. Click ?? Low Stock Filter button
2. Button turns orange
3. Table shows only low stock items
4. See count in button
```

### What's "Low Stock"?
```
Any part below its minimum level
(You set the minimum when adding)
```

### Turn Off Filter
```
1. Click ?? Low Stock Filter again
2. Button turns gray
3. All parts shown again
```

---

## 4?? Add a New Part (2 minutes)

### Step 1: Open Form
```
1. Click ? Add Spare Part button
2. Modal form appears
```

### Step 2: Fill Required Fields (marked with *)
```
Minimum Required:
? Part Number  (e.g., MTR-001)
? Name         (e.g., Motor)
? Category     (e.g., Motors)
? Unit Cost    (e.g., 120)
? Quantity     (e.g., 5)
```

### Step 3: Optional Details
```
Optional:
? Manufacturer
? Supplier
? Location
? Description
? Notes
? Generic (yes/no)
```

### Step 4: Save
```
1. Click Add button
2. Wait for ? (appears at top)
3. Modal closes
4. New part appears in table
```

---

## 5?? Record a Transaction (1 minute)

### Step 1: Open Transaction Form
```
Option A: Click ?? Record Transaction
Option B: Find part, click ?? Issue
```

### Step 2: Fill Form

**If using Record Transaction:**
```
Required:
? Select Part       (from dropdown)
? Select Type       (Issue/Restock/Return/Adjust)
? Quantity          (how many)

Optional:
? Work Order        (link to job)
? Issued To         (who received)
? Reason            (why)
```

**If using Quick Issue:**
```
Just set:
? Work Order        (optional)
? Quantity          (usually 1)
```

### Step 3: Save
```
1. Click Record Transaction
2. Wait for ? success message
3. Check Recent Transactions table
4. Quantity updated in parts list
```

---

## ?? Common Tasks

### View Part Details
```
1. Find part in table
2. Click ??? View button
3. See all info
4. See transaction history
5. Click Close or click outside modal
```

### Edit a Part
```
1. Find part in table
2. Click ?? Edit button
3. Modal opens with part data
4. Change what you need
5. Click Update
6. See ? success message
```

### Filter by Category
```
1. Use "Category" dropdown
2. Select Bearings, Motors, Seals, etc.
3. Table updates instantly
4. Select "All" to clear
```

### Refresh Data
```
1. Click ?? Refresh button
2. See loading animation
3. Data reloads from database
4. See ? refresh complete message
```

---

## ? Verification

### Check It's Working

```
1. See page loads        ?
2. See parts in table    ?
3. See stat cards        ?
4. Click buttons work    ?
5. Modals open/close     ?
6. Notifications appear  ?
```

### If Something's Wrong

```
Issue: Can't add parts
? Check if you have Editor role
? Contact administrator

Issue: Can't see data
? Try clicking Refresh
? Try logging out and in

Issue: Error message
? Read the message
? Contact administrator with error text
```

---

## ?? Dashboard Stats Explained

### 4 Main Stats Cards

```
?? Total Parts
? How many parts you have
? Counts all parts

?? Low Stock
? How many below minimum
? Click filter to see them

?? Total Value
? What inventory is worth
? (Quantity × Unit Cost)

?? Transactions
? Recent activities
? Last 10 transactions
```

---

## ?? Transaction Types

### 4 Types Available

```
?? Issue
? You're using a part
? Quantity goes DOWN
? (e.g., installed in equipment)

?? Restock
? You received more parts
? Quantity goes UP
? (e.g., purchase arrived)

?? Return
? Parts being returned
? Quantity goes UP
? (e.g., unused parts back)

?? Adjustment
? Inventory correction
? Can go UP or DOWN
? (e.g., found missing parts)
```

---

## ?? Color Meanings

### Status Badges

```
? In Stock (Green)
? Have enough parts
? No action needed

?? Low Stock (Orange)
? Getting low
? Consider ordering

? Out of Stock (Red)
? No parts available
? Urgent order needed
```

### Messages

```
? (Green) Success
? Operation completed

?? (Red/Orange) Warning
? Validation error
? Required field missing

?? (Blue) Info
? Additional information
```

---

## ?? Permissions

### What Can You Do?

**If You're Viewing Only:**
```
? Can view all parts
? Can view details
? Can view transactions
? Cannot add parts
? Cannot edit parts
? Cannot record transactions
```

**If You're an Editor:**
```
? Can do everything above
? Can add parts
? Can edit parts
? Can record transactions
? Can issue parts
```

---

## ?? Quick Tips

### Pro Tips
```
1. Use filters to find parts quickly
2. Set accurate minimum levels
3. Record transactions immediately
4. Link work orders for tracking
5. Review low stock weekly
6. Keep descriptions helpful
7. Use categories consistently
```

### Best Practices
```
1. Add parts before you need them
2. Keep quantities updated
3. Link to work orders
4. Add clear descriptions
5. Use consistent naming
6. Monitor low stock
7. Review transactions regularly
```

---

## ?? Need Help?

### Quick Reference
- **Common Tasks**: SPARE_PARTS_QUICK_REFERENCE.md
- **Full Guide**: SPARE_PARTS_PRODUCTION_READY.md
- **Troubleshooting**: See "Common Tasks" section below

### Contact
- **Admin Issues**: Contact your system administrator
- **Permission Issues**: Ask for Editor role
- **Data Issues**: Click Refresh or contact admin

---

## ?? Troubleshooting Quick Fixes

### Problem: "Add button is disabled"
**Solution:** You need Editor role. Contact admin.

### Problem: "Can't find the page"
**Solution:** Use URL: /rbm/spare-parts

### Problem: "Modal won't open"
**Solution:** Refresh page. Try again.

### Problem: "Data not updating"
**Solution:** Click ?? Refresh button.

### Problem: "Error message appeared"
**Solution:** Read message. Contact admin if unsure.

---

## ? Next Steps

### After 5 Minutes

1. **Explore the Page**
   - View all parts
   - Filter by category
   - Check transactions

2. **Add a Test Part**
   - Click Add
   - Fill form
   - Click Save

3. **Record a Test Transaction**
   - Click Record Transaction
   - Select part
   - Enter quantity
   - Save

4. **Verify Everything Works**
   - Check if part appears
   - Check if quantity updated
   - Check if transaction shown

5. **Read More**
   - Open SPARE_PARTS_QUICK_REFERENCE.md
   - Review the full guide
   - Learn all features

---

## ?? Documentation

### For Different Needs

```
5 Min Overview:      This document ?
Quick Reference:     SPARE_PARTS_QUICK_REFERENCE.md
Complete Guide:      SPARE_PARTS_PRODUCTION_READY.md
Visual Guide:        SPARE_PARTS_VISUAL_GUIDE.md
Troubleshooting:     SPARE_PARTS_QUICK_REFERENCE.md
Index:               SPARE_PARTS_INDEX.md
```

---

## ?? You're Ready!

### You Can Now

? Access the Spare Parts page
? View all parts and transactions
? Filter and search parts
? Add new parts (if authorized)
? Record transactions (if authorized)
? Manage inventory
? Monitor stock levels

---

## ?? Next Level

### When You're Ready

- Read full production guide
- Learn advanced filtering
- Set up categories
- Configure stock levels
- Monitor trends
- Use reporting features
- Train other users

---

## ?? Questions?

### Common Questions

**Q: How do I know if I have permission?**
A: Try adding a part. If button works, you have permission.

**Q: How do I find a specific part?**
A: Use filters or scroll through table. (Full search coming)

**Q: Where do I see all transactions?**
A: Scroll down to Recent Transactions section.

**Q: Can I undo a transaction?**
A: Not yet. Be careful entering data.

**Q: How do I delete a part?**
A: Contact administrator. (Delete feature coming)

---

## ? Checklist: You're Ready When...

- [ ] Page loads without errors
- [ ] You can see parts in table
- [ ] You can filter parts
- [ ] You can see transaction table
- [ ] Buttons respond to clicks
- [ ] Modals open and close
- [ ] You can view part details
- [ ] You understand the stats
- [ ] You know your permissions
- [ ] You're ready to start using!

---

## ?? Welcome to Spare Parts!

You're now ready to use the **Spare Parts Inventory Management** system.

**Enjoy!** ??

---

**Quick Links**
- ?? [Quick Reference](SPARE_PARTS_QUICK_REFERENCE.md)
- ?? [Full Guide](SPARE_PARTS_PRODUCTION_READY.md)
- ?? [Visual Guide](SPARE_PARTS_VISUAL_GUIDE.md)
- ?? [All Docs](SPARE_PARTS_INDEX.md)

---

**Version:** 2.0
**Date:** December 15, 2024
**Status:** ? PRODUCTION READY
