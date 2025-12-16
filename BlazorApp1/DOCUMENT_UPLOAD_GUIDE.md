# Document Upload Feature - Quick Start Guide

## ? Upload Functionality Added

The Documents page now has **complete upload and edit capabilities**!

### Features Added

1. **Upload Modal** ??
   - Click "?? Upload Document" button
   - Complete form with document metadata
   - Select file (max 50MB)
   - Link to assets or work orders
   - Set expiry/review dates
   - Add tags for easy searching

2. **Edit Modal** ??
   - Click ?? edit button on any document
   - Update metadata (can't change file in edit mode)
   - Update version/revision numbers
   - Change status, dates, links

3. **View Modal** ???
   - Click ??? view button
   - See complete document details
   - View access history
   - See linked assets/work orders

### File Upload Details

**Supported Formats:**
- PDF (.pdf)
- Word (.doc, .docx)
- Excel (.xls, .xlsx)
- Images (.jpg, .jpeg, .png)
- CAD (.dwg)

**Max File Size:** 50MB

**Upload Location:** `wwwroot/uploads/documents/`

**File Naming:** Files are saved with GUID prefix to prevent overwrites:
- Original: `Manual_PMP-001.pdf`
- Saved as: `a1b2c3d4-e5f6-..._Manual_PMP-001.pdf`

### Form Fields

**Required Fields:**
- Document Number (auto-generated)
- Title
- Category
- File (for new uploads)

**Optional Fields:**
- Description
- Sub-Category
- Status (Draft, Active, etc.)
- Version & Revision
- Link to Asset
- Link to Work Order
- Author & Department
- Effective/Expiry/Review Dates
- Tags
- Access Level
- Notes

### Upload Process

1. **Click Upload Button**
   ```
   ?? Upload Document
   ```

2. **Fill in Document Info**
   - Title: "Hydraulic Pump Manual"
   - Category: "Manual"
   - Sub-Category: "Operation Manual"

3. **Select File**
   - Click "Choose File"
   - Select your PDF/document
   - See confirmation: "? Selected: filename.pdf (2.3 MB)"

4. **Link to Asset (Optional)**
   - Select from dropdown
   - Links document to specific equipment

5. **Set Dates (Optional)**
   - Effective Date: When document becomes active
   - Expiry Date: When document expires
   - Review Date: When document needs review

6. **Add Tags**
   - maintenance, pump, safety
   - Helps with searching later

7. **Click Upload**
   - File uploads to server
   - Document saved to database
   - Success message shows
   - Modal closes automatically

### Upload Directory Setup

**IMPORTANT:** The upload directory must exist!

Create the directory structure:
```
wwwroot/
  ??? uploads/
      ??? documents/
```

**PowerShell Command:**
```powershell
New-Item -Path "wwwroot\uploads\documents" -ItemType Directory -Force
```

**Or manually:**
1. Go to `BlazorApp1` folder
2. Go to `wwwroot` folder
3. Create `uploads` folder
4. Inside `uploads`, create `documents` folder

### Validation

The system validates:
- ? Title is required
- ? Category is required
- ? File is required (for new uploads)
- ? File size <= 50MB
- ? File format is supported

Error messages show in red if validation fails.

### Success Messages

- **Upload:** "Document uploaded successfully!"
- **Edit:** "Document updated successfully!"
- **Error:** "Error: [error message]"

### Access Logging

Every action is logged:
- **View**: When someone views document details
- **Download**: When someone downloads the file
- **Edit**: When document metadata is updated
- **Delete**: When document is removed

View the access log in the document details modal.

### Linking Documents

**Link to Asset:**
- Useful for equipment manuals, specs, photos
- Select asset from dropdown
- Document shows up when viewing that asset

**Link to Work Order:**
- Useful for completion photos, reports
- Select work order from dropdown
- Document shows up with that work order

**No Link (Generic):**
- Company-wide SOPs
- General policies
- Available to all

### Document Lifecycle

1. **Upload** ? Status: Draft
2. **Review** ? Status: Under Review
3. **Approve** ? Status: Approved
4. **Activate** ? Status: Active
5. **Archive** ? Status: Archived
6. **Mark Obsolete** ? Status: Obsolete

### Finding Your Documents

**Filter by Category:**
- Select category from dropdown
- See only manuals, SOPs, drawings, etc.

**Filter by Status:**
- Select status from dropdown
- See only active, draft, archived, etc.

**Search:**
- Type in search box
- Searches title, description, tags, doc number

**Quick Filters:**
- **?? Expired Only**: Shows expired documents
- **? Needs Review**: Shows documents due for review

### Editing Documents

1. Click ?? edit button
2. Modal opens with current data
3. Modify metadata (file cannot be changed)
4. Click "?? Update"
5. Changes saved

**Note:** You cannot change the uploaded file in edit mode. To change the file, you must upload a new document with a new version number.

### Deleting Documents

1. Click ??? delete button
2. Document removed from database
3. Physical file deleted from server
4. **Cannot be undone!**

**Permission required:** Only users with Delete permission can delete documents.

### Troubleshooting

**"Please select a file to upload!"**
- Solution: Click "Choose File" and select a document

**"File size exceeds 50MB limit!"**
- Solution: Compress the file or split into multiple documents

**"Error: [file path error]"**
- Solution: Make sure `wwwroot/uploads/documents` directory exists

**"Upload button is disabled"**
- Solution: Fill in required fields (Title, Category) and select a file

**File upload takes long time**
- Normal for large files (50MB)
- Upload progress shows "? Uploading..."
- Wait for success message

### Version Control

When uploading a new version:
1. Upload new document
2. Increment version number (1.0 ? 1.1 or 2.0)
3. Increment revision number
4. Link to same asset
5. Mark old document as "Archived"

### Best Practices

1. **Use consistent naming**: "AssetID_DocumentType_Description"
2. **Add tags**: Makes searching easier
3. **Set review dates**: Get reminders when review is due
4. **Link to assets**: Easy to find equipment docs
5. **Set access levels**: Restrict sensitive documents
6. **Use categories correctly**: Helps organization
7. **Add descriptions**: Brief summary of contents
8. **Track versions**: Keep version history clear

### Example: Uploading Equipment Manual

```
Title: PMP-001 Hydraulic Pump Operation Manual
Category: Manual
Sub-Category: Operation Manual
File: PMP-001_Manual.pdf (2.5 MB)
Link to Asset: PMP-001 - Hydraulic Pump A
Version: 1.0
Revision: 1
Author: Manufacturer
Department: Maintenance
Effective Date: [today]
Review Date: [6 months from now]
Tags: pump, hydraulic, operation, maintenance
Access Level: Public
Status: Active
```

Result: Document uploaded, linked to pump, searchable, trackable!

---

## ?? Quick Actions

**Upload a Document:**
1. Click "?? Upload Document"
2. Fill form
3. Select file
4. Click "?? Upload"

**Edit a Document:**
1. Click ?? on document row
2. Modify fields
3. Click "?? Update"

**View Document:**
1. Click ??? on document row
2. See full details
3. View access history

**Delete Document:**
1. Click ??? on document row
2. Confirm deletion
3. Document removed

---

## ? You're Ready!

The document upload system is fully functional. Start uploading your manuals, SOPs, drawings, and technical documents!

**Remember:** Create the upload directory first!

```powershell
New-Item -Path "wwwroot\uploads\documents" -ItemType Directory -Force
```

Then restart your application and navigate to `/rbm/documents` to start uploading! ????
