# RBM CMMS Quick Start Guide

## ?? Accessing the Application

1. Run the AppHost project (it should be set as the startup project)
2. When the Aspire dashboard opens, click on the main BlazorApp1 endpoint
3. Navigate to `/rbm` in the URL to access the RBM CMMS Dashboard

## ?? Main Features Tour

### Dashboard (`/rbm`)
**What to explore:**
- View 4 KPI cards showing total assets, MTBF, MTTR, and open work orders
- Check the asset health breakdown (Healthy, Warning, Critical)
- See the top 10 critical assets by health score
- Review upcoming maintenance tasks

### Assets (`/rbm/assets`)
**What to try:**
- Browse the complete asset list with health scores
- Click "??? View" on any asset to see detailed information
- Click "? Add Asset" to create a new asset
- Notice the color-coded criticality badges (Critical = Red, High = Orange, etc.)

**Asset Details Page:**
- View health metrics and uptime/downtime statistics
- See condition history charts
- Review linked work orders for that specific asset
- Check FMEA summary

### Condition Monitoring (`/rbm/condition-monitoring`)
**What to try:**
- Select an asset from the dropdown
- Enter condition readings (Temperature, Vibration, Pressure, Oil Analysis)
- Click "?? Save Reading" to log the data
- Observe the health score indicator update
- Read AI-generated recommendations based on the asset's health

### Failure Modes - FMEA (`/rbm/failure-modes`)
**What to try:**
- Review the FMEA table sorted by RPN (Risk Priority Number)
- Notice how RPN is auto-calculated: Severity × Occurrence × Detection
- Check the risk matrix heatmap visualization
- Click "? Add Failure Mode" to create a new entry
- Try the "?? View Risk Matrix" button

### Work Orders (`/rbm/work-orders`)
**What to try:**
- Filter by Status or Priority using the dropdowns
- Click "? Create Work Order"
- Select an asset and link to a failure mode
- Set priority, type, due date, and assign to a technician
- Use "?? Start" to move a work order to "In Progress"
- Use "? Complete" to close a work order
- Notice overdue alerts (red "?? OVERDUE" tag)

### Maintenance Planning (`/rbm/maintenance-planning`)
**What to try:**
- Toggle between Calendar and Gantt views
- Navigate months in calendar view (? Previous / Next ?)
- View technician allocation statistics
- Click "? Add Schedule" to schedule new maintenance
- Review upcoming maintenance table (next 30 days)

### Reliability Analytics (`/rbm/analytics`)
**What to explore:**
- Review MTBF and MTTR trend placeholders
- Check the Pareto analysis of top failure causes
- Read AI-powered predictive maintenance recommendations
- View equipment availability by location
- Try the "?? Export Report" button (UI only)

### Technicians Portal (`/rbm/technicians`)
**What to try:**
- Select a technician from the dropdown (e.g., John Smith, Mike Davis)
- View their assigned work orders
- Click "?? Start Work" to begin a task
- Click "?? Pause" to pause work
- Click "? Complete" to finish
- Check completed tasks for today
- Notice the performance stats at the bottom

### User Management (`/rbm/users`)
**What to try:**
- Review all users in the system
- Check different roles: Admin, Reliability Engineer, Planner, Technician
- Click "? Invite User" to add a new user
- Select a role and see how permissions auto-populate
- View the role descriptions table

### Settings (`/rbm/settings`)
**What to configure:**
- Set condition monitoring thresholds (Temperature, Vibration, Pressure)
- Configure email and SMS notification preferences
- View external integrations (IoT, SCADA, Email, Cloud Backup)
- Change theme (Light mode currently, Dark mode coming soon)
- Try backup/restore and export options

## ?? Interactive Features to Test

### Real-time Updates
1. Create a work order ? See it appear in the Dashboard
2. Change work order status ? Watch counters update
3. Add a condition reading ? Health score updates

### Data Flow
1. **Asset** ? Links to ? **Work Orders** and **Failure Modes**
2. **Failure Mode** ? Calculates ? **RPN** ? Drives **Risk Priority**
3. **Condition Readings** ? Influence ? **Health Score** ? Trigger **Recommendations**
4. **Work Orders** ? Assigned to ? **Technicians** ? Appear in **Technician Portal**

## ?? Visual Elements to Notice

### Color Coding
- **Green**: Healthy assets, low priority, completed items
- **Orange/Yellow**: Warning status, medium priority
- **Red**: Critical status, high priority, overdue items
- **Blue**: In progress, selected items

### Interactive Elements
- Hover over cards to see elevation effects
- Click badges and buttons for actions
- Modal dialogs for data entry (clean, centered overlays)
- Responsive tables with alternating row highlights

## ?? Sample Scenarios to Explore

### Scenario 1: High-Risk Asset Management
1. Go to Dashboard ? Notice "Air Compressor C" has health score of 45 (Critical)
2. Navigate to Assets ? Find and view Air Compressor C details
3. Check linked work orders (should have a critical corrective work order)
4. Go to Failure Modes ? See high RPN items for this asset
5. Go to Analytics ? See predictive alert recommending immediate action

### Scenario 2: Preventive Maintenance Workflow
1. Go to Condition Monitoring ? Select "Centrifugal Pump A"
2. Enter routine readings (all normal values)
3. See health score remains high (85)
4. Go to Maintenance Planning ? Schedule next preventive maintenance
5. Go to Work Orders ? Create preventive work order for next month

### Scenario 3: Technician Daily Work
1. Go to Technicians Portal ? Select "John Smith"
2. View assigned work orders (should see 2-3 tasks)
3. Start work on an open task
4. Complete the work
5. Check "Completed Today" section
6. Review performance stats

## ?? Things to Look For

### Dashboard
- ? 10 total assets displayed
- ? Asset health pie chart breakdown
- ? Upcoming work orders table (5 items)
- ? Top 10 critical assets by health score

### Navigation
- ? Persistent sidebar with all 10 menu items
- ? Active page highlighting in sidebar
- ? Search bar in top navbar
- ? Notification bell with badge (3 notifications)
- ? User avatar and name in top-right

### Responsive Design
- ? Clean card-based layout
- ? Grid systems (2, 3, 4 column grids)
- ? Rounded corners on all elements
- ? Consistent spacing and padding
- ? Professional typography

## ?? Key Metrics in Sample Data

- **Total Assets**: 10
- **Critical Assets**: 2 (Air Compressor C, Boiler F)
- **Warning Assets**: 3 (Electric Motor B, Control Valve E, Gearbox I)
- **Healthy Assets**: 5
- **Open Work Orders**: 5
- **Critical Work Orders**: 2
- **Users**: 5 (1 Admin, 1 Engineer, 2 Technicians, 1 Planner)
- **Failure Modes**: 5 with RPN ranging from 60 to 216

## ?? Getting the Most from the Demo

1. **Start at the Dashboard** - Get the big picture
2. **Explore Assets** - Understand the equipment portfolio
3. **Check Work Orders** - See maintenance activities
4. **Review Analytics** - Learn from data insights
5. **Try the Technician Portal** - Experience the mobile-friendly view
6. **Visit Settings** - See configuration options

Enjoy exploring the RBM CMMS platform! ??
