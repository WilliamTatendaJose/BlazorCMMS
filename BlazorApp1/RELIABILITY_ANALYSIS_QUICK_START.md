# ?? Reliability Analysis - Quick Start Guide

## ?? Get Started in 5 Minutes

### Step 1: Access the Feature
```
Navigate to: /rbm/reliability-analysis
```

### Step 2: View Fleet Overview
You'll see:
- **Fleet Availability:** Overall system uptime %
- **Average MTBF:** Mean time between failures
- **Average MTTR:** Mean time to repair
- **Average OEE:** Overall equipment effectiveness
- **Critical Assets:** Count of underperforming assets

### Step 3: Filter Data
Select from dropdowns:
1. **Period:** Daily/Weekly/Monthly/Quarterly/Yearly
2. **Filter:** All/Critical/Low Availability/High Downtime
3. Click **Recalculate**

### Step 4: View Asset Details
Click **??? View** on any asset to see:
- ? Detailed metrics (MTBF, MTTR, Availability, OEE)
- ?? 12-month trend charts
- ?? Recent downtime events
- ?? Failure modes
- ?? Performance recommendations

### Step 5: Export Report
Click **?? Export Report** to download analysis as CSV/PDF

---

## ?? Understanding the Metrics

### MTBF (Mean Time Between Failures)
**What:** Average hours of operation between failures
**Example:** 1000 hours = asset typically fails after 1000 hours
**Target:** > 500 hours (higher is better)
**Action:** If low, investigate failure patterns

### MTTR (Mean Time To Repair)
**What:** Average hours needed to fix asset after failure
**Example:** 4 hours = repairs typically take 4 hours
**Target:** < 5 hours (lower is better)
**Action:** If high, improve maintenance procedures or spares availability

### Availability
**What:** Percentage of time asset is operational
**Example:** 95% = asset is working 95% of the time
**Target:** > 95% (higher is better)
**Action:** If low, analyze downtime causes

### OEE (Overall Equipment Effectiveness)
**What:** Combined metric of availability, performance, and quality
**Example:** 92% = very good performance
**Target:** > 85% (higher is better)
**Action:** Benchmark against similar equipment

---

## ?? Common Tasks

### Task 1: Record Equipment Downtime

1. Navigate to **Assets** ? Select Asset
2. In Asset Details, look for "Recent Downtime Events"
3. Click **? Add Downtime Event**
4. Fill in:
   - **Start Time:** When did downtime begin?
   - **End Time:** When did equipment restart?
   - **Reason:** Breakdown/Maintenance/Setup
   - **Category:** Planned/Unplanned
   - **Production Loss:** How many units not produced?
   - **Financial Impact:** Estimated cost

5. Click **Save**

### Task 2: Analyze Asset Performance Decline

1. Go to **Reliability Analysis**
2. Click **View** on declining asset
3. Check **Historical Trends** section
4. Look for:
   - Decreasing MTBF trend?
   - Increasing MTTR trend?
   - Declining Availability?
5. Review **Recent Downtime Events**
6. Note failure patterns
7. Compare **Failure Modes** to identify issues
8. Create **Work Order** for maintenance

### Task 3: Generate Monthly Report

1. Select Period: **Monthly**
2. Click **Recalculate**
3. Review **Reliability Metrics Table**
4. Check **OEE Performance Tiers** pie chart
5. Review **Top Reliability Issues**
6. Click **?? Export Report**
7. Share with management

### Task 4: Find Critical Assets

1. Select Filter: **Critical Assets Only**
2. Click **Recalculate**
3. View filtered table
4. For each asset, click **View**
5. Review performance recommendations
6. Plan corrective actions
7. Link to maintenance requests

---

## ?? Alert Indicators

| Icon | Meaning | Action |
|------|---------|--------|
| ?? | Excellent (OEE > 85%) | Maintain current maintenance |
| ?? | Good (70% - 85% OEE) | Monitor for changes |
| ?? | Poor (OEE < 70%) | Review maintenance strategy |
| ?? | Critical | Urgent maintenance needed |
| ?? | Low Availability | Analyze downtime |

---

## ?? Pro Tips

### Tip 1: Spot Trends Early
- Review monthly trends religiously
- Compare current month to last 3 months
- Alert team if trend is negative

### Tip 2: Link Data
- Always link downtime to work orders
- Record detailed descriptions
- Include production impact
- Track financial cost

### Tip 3: Use Filters Effectively
- **Critical Assets:** Focus on high-impact equipment
- **Low Availability:** Find problem children
- **High Downtime:** Identify planned maintenance opportunities

### Tip 4: Benchmark Assets
- Compare similar equipment
- Identify best performers
- Study their maintenance approach
- Share best practices

### Tip 5: Export Regularly
- Export weekly for your team
- Export monthly for management
- Export quarterly for board
- Keep historical records

---

## ? Quick FAQs

**Q: How often should I check reliability metrics?**
A: At least weekly. Critical assets should be monitored daily.

**Q: What if availability is declining?**
A: Record all downtime events, identify patterns, and increase PM frequency.

**Q: How do I improve MTTR?**
A: Ensure spare parts availability, train technicians, improve procedures.

**Q: What's a good MTBF?**
A: Depends on equipment type. Target is usually 500+ hours. Compare with similar assets.

**Q: How is OEE calculated?**
A: OEE = Availability × Performance × Quality. Higher is better.

**Q: Can I see historical trends?**
A: Yes! Each asset shows 12-month trend charts when you click View.

**Q: How do I export the report?**
A: Click ?? Export Report button. Downloads as CSV/PDF.

**Q: What causes low availability?**
A: Planned maintenance, unplanned failures, setup time, or repair delays.

**Q: Should I worry if OEE drops 5%?**
A: Monitor it. If 10%+ drop, investigate immediately.

**Q: How do I find problem assets?**
A: Filter by "Critical Assets Only" or "Low Availability".

---

## ?? Next Steps

After mastering Reliability Analysis:

1. **Explore Asset Management**
   - Link downtime to assets
   - Track maintenance history
   - Plan preventive maintenance

2. **Master Work Orders**
   - Create corrective maintenance
   - Link to downtime events
   - Track repair costs

3. **Analyze Failure Modes**
   - Identify common failures
   - Use FMEA for analysis
   - Plan preventive actions

4. **Monitor Conditions**
   - Track temperature, vibration, pressure
   - Set alerts for anomalies
   - Use for predictive maintenance

5. **Manage Spare Parts**
   - Ensure critical parts in stock
   - Reduce repair time (MTTR)
   - Plan inventory

---

## ?? Need Help?

- **I can't find my asset:** Use search box at top
- **Metrics look wrong:** Click Recalculate button
- **Want to record downtime:** Go to Assets page, find asset, add downtime
- **Need to export data:** Click Export Report button
- **Still stuck:** Contact your Reliability Engineer

---

## ? Checklist: First Week

- [ ] Access Reliability Analysis page
- [ ] Review fleet overview metrics
- [ ] Check your critical assets
- [ ] Record one downtime event
- [ ] Click View on an underperforming asset
- [ ] Review historical trends
- [ ] Filter by "Critical Assets Only"
- [ ] Export a report
- [ ] Share findings with team
- [ ] Plan follow-up actions

---

**Status:** ? Ready to Use

**Version:** 1.0

**Last Updated:** December 15, 2024
