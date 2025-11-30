# RBM CMMS - Reliability-Based Maintenance Platform

A comprehensive, full-featured Computerized Maintenance Management System (CMMS) built with Blazor, focused on reliability-based maintenance strategies for industrial equipment.

## ?? Overview

The RBM CMMS platform is designed for engineers, maintenance planners, and reliability analysts to proactively manage equipment health, predict failures, and optimize maintenance schedules based on risk analysis.

## ? Key Features

### 1. **Dashboard**
- Real-time asset health overview with visual indicators
- KPI cards for MTBF, MTTR, and active work orders
- Top 10 critical assets ranked by health score
- Upcoming maintenance tasks calendar view
- Condition trend charts for monitoring

### 2. **Asset Management**
- Comprehensive asset database with criticality ratings
- Health score tracking (0-100 scale)
- Asset detail pages with:
  - Performance metrics (uptime/downtime)
  - Condition history trends
  - Linked work orders
  - FMEA summaries
- Add, edit, and delete assets

### 3. **Condition Monitoring**
- Manual data entry for:
  - Temperature readings
  - Vibration analysis
  - Pressure measurements
  - Oil analysis results
- Real-time health score indicator
- AI-powered recommendations based on condition data
- Trend visualization for predictive insights

### 4. **Failure Modes (FMEA)**
- Full Failure Mode and Effects Analysis implementation
- Auto-calculated Risk Priority Number (RPN = S × O × D)
- Interactive risk matrix heatmap visualization
- Prioritized failure mode table
- One-click maintenance plan generation from high-risk items

### 5. **Work Order Management**
- Create, assign, and track work orders
- Priority-based filtering (Critical, High, Medium, Low)
- Status tracking (Open, In Progress, Completed)
- Link work orders to specific failure modes
- Overdue alerts and notifications
- Color-coded priority badges

### 6. **Maintenance Planning**
- Dual view modes:
  - **Calendar View**: Monthly maintenance schedule
  - **Gantt View**: 30-day timeline visualization
- Technician allocation dashboard
- Auto-schedule by risk feature
- Drag-and-drop reschedule capability (planned)
- Resource optimization

### 7. **Reliability Analytics**
- MTBF and MTTR trend analysis
- Pareto charts for top failure causes
- Equipment availability and reliability KPIs
- AI-powered predictive maintenance alerts
- Location-based performance metrics
- Export reports to Excel/CSV

### 8. **Technicians Portal**
- Mobile-friendly interface
- QR code scanning for asset check-in
- Assigned work order list
- Quick actions: Start, Pause, Complete
- Offline sync status indicator
- Daily completion tracker
- Performance statistics

### 9. **User Management**
- Role-based access control:
  - **Admin**: Full system access
  - **Reliability Engineer**: FMEA and analytics
  - **Planner**: Work orders and schedules
  - **Technician**: Execute tasks and record data
- Granular permissions (View/Edit/Delete)
- User invitation system
- Last login tracking

### 10. **Settings & Integrations**
- Configurable condition thresholds
- Email and SMS notification preferences
- External integrations:
  - IoT Platform (sensor data)
  - SCADA Systems
  - Email Service (SMTP)
  - Cloud Backup (Azure Blob Storage)
- Theme selector (Light/Dark)
- Data backup and restore
- System information dashboard

## ?? Design System

### Industrial Blue-Grey Theme
- **Primary Colors**: 
  - Blue-Grey (`#37474f`, `#546e7a`, `#263238`)
  - Accent Blue (`#0288d1`, `#03a9f4`)
- **Status Colors**:
  - Success Green: `#43a047`
  - Warning Orange: `#fb8c00`
  - Danger Red: `#e53935`
- **Typography**: Inter/Roboto font family
- **Components**: Rounded cards, smooth transitions, clean layout

### UI Components
- Persistent sidebar navigation with icons
- Top navbar with search, notifications, user profile
- Data cards with hover effects
- Responsive tables with sorting
- Modal dialogs for forms
- Color-coded badges and tags
- Interactive charts and visualizations

## ?? Getting Started

### Prerequisites
- .NET 10 SDK
- Visual Studio 2022 or VS Code
- SQL Server (for production use)

### Running the Application

1. **Navigate to the AppHost project**:
   ```bash
   cd BlazorApp1.AppHost
   ```

2. **Run the application**:
   ```bash
   dotnet run
   ```

3. **Access the RBM CMMS**:
   - Open browser and navigate to the provided localhost URL
   - Click on the main application link
   - Navigate to `/rbm` to access the RBM CMMS dashboard

### Navigation Structure

```
/rbm                          ? Dashboard
/rbm/assets                   ? Asset Management
/rbm/assets/{id}              ? Asset Details
/rbm/condition-monitoring     ? Condition Monitoring
/rbm/failure-modes            ? FMEA Analysis
/rbm/work-orders              ? Work Order Management
/rbm/maintenance-planning     ? Maintenance Planning
/rbm/analytics                ? Reliability Analytics
/rbm/technicians              ? Technicians Portal
/rbm/users                    ? User Management
/rbm/settings                 ? Settings & Integrations
```

## ?? Sample Data

The application comes pre-loaded with sample data for demonstration:
- **10 Assets** across 3 buildings with varying health scores
- **6 Work Orders** in different states
- **5 Failure Modes** with calculated RPN values
- **5 Users** representing different roles
- **Condition Readings** with temperature, vibration, and pressure data
- **Maintenance Schedules** for the next 30 days

## ?? Technical Architecture

### Technology Stack
- **Frontend**: Blazor Server (.NET 10)
- **Styling**: Custom CSS with CSS Variables
- **State Management**: In-memory data service (singleton)
- **Layout**: Sidebar + Main content area
- **Interactivity**: Server-side rendering with real-time updates

### Project Structure
```
BlazorApp1/
??? Components/
?   ??? Layout/
?   ?   ??? RBMLayout.razor          # Main RBM layout
?   ??? Pages/
?       ??? RBM/                     # All RBM pages
??? Models/                          # Data models
?   ??? Asset.cs
?   ??? ConditionReading.cs
?   ??? FailureMode.cs
?   ??? WorkOrder.cs
?   ??? User.cs
?   ??? MaintenanceSchedule.cs
??? Services/
?   ??? DataService.cs               # Data management service
??? wwwroot/
    ??? css/
        ??? rbm-styles.css           # RBM-specific styles
```

## ?? Target Users

- **Maintenance Engineers**: Equipment reliability and FMEA analysis
- **Maintenance Planners**: Work order scheduling and resource allocation
- **Reliability Analysts**: Performance metrics and predictive insights
- **Technicians**: Field work execution and data collection
- **Plant Managers**: Overall asset health visibility

## ?? Future Enhancements

- Real-time IoT sensor integration
- Machine learning for failure prediction
- Mobile app for technicians
- Advanced charting with Chart.js/ApexCharts
- Document management for manuals and procedures
- Inventory management for spare parts
- Cost tracking and ROI analysis
- Multi-site/multi-plant support
- API for third-party integrations

## ?? Notes

- Currently uses in-memory data storage (singleton service)
- For production, integrate with Entity Framework and SQL Server
- Authentication is available but not enforced on RBM routes
- Charts show placeholder visualizations - can be enhanced with charting libraries

## ?? License

This is a prototype/demonstration application for educational purposes.

---

**Built with ?? using Blazor and .NET 10**
