# Documents Upload & View - Visual Guide ??

## Quick Overview

### What You Can Now Do

? **Upload Documents**
- Click button ? Fill form ? Select file ? Upload ?

? **View Details**
- Click view icon ? See full info ? Check history ??

? **Edit Documents**
- Click edit icon ? Modify metadata ? Update ??

---

## Step-by-Step Guide

### ?? Uploading a Document

**Step 1: Click Upload Button**
```
???????????????????????????
? ?? Upload Document     ? ? Click here
? ?? Expired Only         ?
? ? Needs Review         ?
???????????????????????????
```

**Step 2: Modal Opens**
```
????????????????????????????????????????
? ?? Upload Document          [×]     ?
????????????????????????????????????????
? Document Number: DOC-2024-001        ?
? Title: *[required]                   ?
? Description: [optional]              ?
????????????????????????????????????????
```

**Step 3: Fill Required Fields**
```
Required:
? Title (document name)
? Category (Manual, SOP, Drawing, etc.)
? File (select file to upload)

Optional:
? Description
? Sub-Category
? Version/Revision
? Dates
? Tags
```

**Step 4: Select File**
```
[Choose File]
        ?
? Selected: "Pump_Manual.pdf" (2.3 MB)
        ?
    Upload!
```

**Step 5: Click Upload**
```
????????????????????????????????????????
? [Cancel] [?? Upload] ? Click        ?
????????????????????????????????????????
        ?
   (Uploading...)
        ?
? Success! Document uploaded.
```

---

### ??? Viewing Document Details

**Step 1: Click View Button**
```
Document List:
? Doc-001 ? Pump Manual ? Active ? [???] [??] [???]
                                   ?
                           Click here
```

**Step 2: View Modal Opens**
```
???????????????????????????????????????????
? ?? DOC-001 - Pump Manual        [×]    ?
???????????????????????????????????????????
?                                         ?
?  [4 Statistics Cards]                   ?
?  ??? 5  ? ?? 2  ? ?? 1.0 ? ?? 2.3 MB  ?
?  Views ? Downloads?Version? File Size  ?
?                                         ?
?  [Document Information]                 ?
?  Category: Manual                       ?
?  Status: Active                         ?
?  File: Pump_Manual.pdf                 ?
?                                         ?
?  [Linked Items]                         ?
?  ?? Asset: PMP-001 - Hydraulic Pump    ?
?                                         ?
?  [Description]                          ?
?  Complete operation manual for pump...  ?
?                                         ?
?  [Access History]                       ?
?  ? User      ? Action ? Date/Time      ?
?  ? John      ? View   ? Dec 01 10:30   ?
?  ? Sarah     ? View   ? Nov 30 14:20   ?
?                                         ?
? [Close]                                 ?
???????????????????????????????????????????
```

---

### ?? Editing a Document

**Step 1: Click Edit Button**
```
Document List:
? Doc-001 ? Pump Manual ? Active ? [???] [??] [???]
                                        ?
                                Click here
```

**Step 2: Edit Modal Opens (Same as Upload)**
```
????????????????????????????????????????
? ?? Edit Document            [×]     ?
????????????????????????????????????????
?                                      ?
? Document Number: DOC-001 (readonly)  ?
? Title: [Pump Manual]                ?
? Category: [Manual]                  ?
? ...existing data...                 ?
?                                      ?
? Current File:                       ?
? Pump_Manual.pdf (2.3 MB)            ?
? (Cannot change in edit mode)         ?
?                                      ?
? [Cancel] [?? Update]                ?
????????????????????????????????????????
```

**Step 3: Modify & Save**
```
Changes Allowed:
? Title
? Description
? Category
? Sub-Category
? Status
? Version/Revision
? Dates (Effective, Expiry, Review)
? Tags
? Notes
? Links (Asset, Work Order)
? Author/Department
? Access Level

Changes NOT Allowed:
? Document Number (auto-generated)
? File (upload new document instead)
```

---

## Form Fields Explained

### Basic Information
| Field | Type | Required | Example |
|-------|------|----------|---------|
| Doc # | Text | ? | DOC-2024-001 |
| Title | Text | ? | Pump Operation Manual |
| Description | Text | ? | Complete operation guide |
| Category | Select | ? | Manual |
| Sub-Category | Text | ? | Operation Manual |
| Status | Select | ? | Active |

### File & Version
| Field | Type | Required | Example |
|-------|------|----------|---------|
| File | Upload | ? (new) | Pump_Manual.pdf |
| Version | Text | ? | 1.0 |
| Revision | Number | ? | 1 |

### Linking
| Field | Type | Required | Notes |
|-------|------|----------|-------|
| Asset | Select | ? | Links to equipment |
| Work Order | Select | ? | Links to maintenance task |

### Organization
| Field | Type | Required | Example |
|-------|------|----------|---------|
| Author | Text | ? | Manufacturer |
| Department | Text | ? | Maintenance |
| Access Level | Select | ? | Public |

### Dates
| Field | Type | Required | Purpose |
|-------|------|----------|---------|
| Effective | Date | ? | When active |
| Expiry | Date | ? | When expires |
| Review | Date | ? | When to review |

### Metadata
| Field | Type | Required | Example |
|-------|------|----------|---------|
| Tags | Text | ? | maintenance, pump |
| Notes | Text | ? | Additional info |

---

## Status Messages

### Success ?
```
Green alert:
? Document uploaded successfully!
  (Modal closes automatically)

? Document updated successfully!
  (Modal closes automatically)
```

### Errors ?
```
Red alert:
? Title is required!
? Category is required!
? Please select a file to upload!
? File size exceeds 50MB limit!
? Error: [specific error message]
```

### Uploading ?
```
Blue alert:
? Uploading...
  (Button disabled during upload)
```

---

## Keyboard Shortcuts

| Key | Action |
|-----|--------|
| `ESC` | Close modal |
| `Tab` | Move to next field |
| `Enter` | Submit form (if focused on button) |
| `Ctrl+A` | Select all (in text fields) |

---

## Tips & Tricks

?? **Pro Tips:**

1. **Auto-Generated Numbers**
   - Document number is automatic
   - Follows format: DOC-YYYY-###
   - Read-only field

2. **File Upload**
   - Max size: 50MB
   - Supported: PDF, DOC, DOCX, XLS, XLSX, JPG, PNG, DWG
   - Shows file size before upload

3. **Categories**
   - Use consistent categories
   - Helps with organization
   - Filter by category in main view

4. **Versioning**
   - Update version when major changes
   - Increment revision for minor changes
   - Archive old versions

5. **Dates**
   - Effective: When document becomes active
   - Expiry: When document expires (if applicable)
   - Review: When to review/update

6. **Tags**
   - Use for better search
   - Comma-separated
   - Examples: "maintenance, pump, safety"

7. **Links**
   - Link to specific assets
   - Helps track equipment docs
   - Appears in asset details

8. **Access Level**
   - Public: Everyone can view
   - Restricted: Certain roles only
   - Confidential: Admin only

---

## Common Workflows

### ?? Upload Equipment Manual
```
1. Click ?? Upload
2. Title: "Equipment Name Manual"
3. Category: Manual
4. Select PDF file
5. Link to Asset: [Select equipment]
6. Status: Active
7. Click Upload ?
```

### ?? Add Safety Procedure
```
1. Click ?? Upload
2. Title: "Safety Procedure Name"
3. Category: SOP
4. Sub-Category: Safety SOP
5. Select PDF file
6. Access Level: Public
7. Status: Active
8. Click Upload ?
```

### ?? Upload Report
```
1. Click ?? Upload
2. Title: "Quarterly Report Q1"
3. Category: Report
4. Select PDF file
5. Department: Reliability
6. Status: Active
7. Tags: "report, quarterly"
8. Click Upload ?
```

### ?? Add Equipment Photo
```
1. Click ?? Upload
2. Title: "Equipment Damage Photo"
3. Category: Photo
4. Select JPG file
5. Link to Asset: [Equipment]
6. Link to Work Order: [WO number]
7. Status: Active
8. Click Upload ?
```

---

## Troubleshooting

### Upload Button Not Showing
- ? Check permissions (CanEdit)
- ? Verify you're logged in
- ? Refresh page

### File Won't Upload
- ? Check file size (max 50MB)
- ? Verify file type supported
- ? Check internet connection
- ? Try different browser

### Form Won't Submit
- ? Fill required fields (Title, Category, File)
- ? Check for error messages
- ? Try smaller file
- ? Refresh and try again

### Modal Won't Close
- ? Click X button
- ? Click Cancel button
- ? Press ESC key
- ? Refresh page

### Fields Not Saving
- ? Wait for success message
- ? Check for error messages
- ? Verify all required fields filled
- ? Check internet connection

---

## Visual Indicators

### Document Status Badges
```
Draft      ? ?? Yellow
Approved   ? ?? Blue
Active     ? ?? Green
Archived   ? ? Grey
Obsolete   ? ?? Red
```

### Alerts
```
? Success  ? ?? Green background
? Error    ? ?? Red background
? Info     ? ?? Blue background
?? Warning  ? ?? Yellow background
```

### Row Highlighting
```
Expired Documents  ? ?? Light red background
Review Due Docs    ? ?? Light orange background
```

---

## Summary

### Upload Process
1. Click button ? 2. Fill form ? 3. Select file ? 4. Click upload ? ? Done!

### View Process
1. Click icon ? 2. See details ? 3. Check history ? 4. Close

### Edit Process
1. Click button ? 2. Modify fields ? 3. Save ? ? Updated!

**Your document management system is ready to use!** ????
