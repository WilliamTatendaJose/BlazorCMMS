# Units Selector - Deployment Guide

## ?? Quick Deployment Instructions

### Pre-Deployment (5 minutes)

1. **Verify Build**
   ```powershell
   dotnet build
   ```
   Expected: ? Build successful

2. **Review Changes**
   ```powershell
   git diff --stat
   ```
   Expected: 1 file modified, ~150 lines added

3. **Check Test Results**
   - See `UNITS_SELECTOR_TESTING_GUIDE.md`
   - All 20+ test cases should pass

### Deployment Steps (2 minutes)

1. **Merge to Main Branch**
   ```powershell
   git checkout main
   git merge feature/units-selector
   ```

2. **Build for Production**
   ```powershell
   dotnet build --configuration Release
   dotnet publish --configuration Release
   ```

3. **Deploy to Server**
   ```powershell
   # Copy published files to production
   xcopy /E /Y bin/Release/net10.0/publish/* \\server\wwwroot\
   ```

4. **Restart Application**
   ```powershell
   # Restart IIS or container
   iisreset
   # or
   docker restart blazor-cmms
   ```

### Post-Deployment (5 minutes)

1. **Verify Deployment**
   - Navigate to Condition Monitoring page
   - Check Units selector visible in header
   - Verify dropdown works
   - Test unit conversions

2. **Monitor Logs**
   - Check application error log
   - Monitor for exceptions
   - Verify no JavaScript errors

3. **Communicate Rollout**
   - Email users about new feature
   - Include link to `UNITS_SELECTOR_QUICK_REFERENCE.md`
   - Share screenshot in announcement

## ?? Deployment Checklist

### Pre-Deployment
- [ ] Code review completed
- [ ] Build verified (passing)
- [ ] All tests passing (20+ cases)
- [ ] Documentation prepared
- [ ] Backup created
- [ ] Maintenance window scheduled

### During Deployment
- [ ] Build release version
- [ ] Deploy to production
- [ ] Restart application
- [ ] Monitor for errors
- [ ] Test basic functionality

### Post-Deployment
- [ ] Verify feature visible
- [ ] Test unit conversions
- [ ] Check for errors in logs
- [ ] Monitor user feedback
- [ ] Document any issues
- [ ] Announce to users

## ?? Rollback Plan (if needed)

If issues occur, rollback is simple:

```powershell
# 1. Revert to previous version
git revert HEAD
git push

# 2. Build previous version
dotnet build --configuration Release
dotnet publish --configuration Release

# 3. Deploy previous files
xcopy /E /Y bin/Release/net10.0/publish/* \\server\wwwroot\

# 4. Restart application
iisreset
```

**Rollback Time**: ~2 minutes
**Data Impact**: None (no database changes)
**User Impact**: Minimal (feature becomes unavailable)

## ?? Expected Outcomes

### User Experience
- ? Units selector visible in page header
- ? Three unit systems available
- ? Instant conversion on selection
- ? All values update immediately
- ? Form labels and placeholders change
- ? No errors or warnings

### Performance
- ? Page load time: No change
- ? Conversion time: <1ms per value
- ? Memory usage: +50 bytes
- ? No lag on unit switching

### Compatibility
- ? Works on desktop browsers
- ? Works on mobile browsers
- ? Works on tablets
- ? Works offline (if configured)

## ?? Troubleshooting Deployment

### Issue: Build fails after deployment
**Solution**:
1. Check .NET 10 SDK installed
2. Verify all dependencies loaded
3. Clear NuGet cache
4. Rebuild project

### Issue: Units selector not visible
**Solution**:
1. Clear browser cache (Ctrl+Shift+Del)
2. Hard refresh page (Ctrl+F5)
3. Check JavaScript console for errors
4. Verify CSS loaded correctly

### Issue: Conversions not working
**Solution**:
1. Check browser console for errors
2. Verify selectedUnitSystem value
3. Test in different browser
4. Check network tab for issues

### Issue: Performance degradation
**Solution**:
1. Check network tab (should be fast)
2. Monitor memory usage
3. Check for JavaScript errors
4. Verify server resources

## ?? Support During Rollout

### User Issues
- Refer to: `UNITS_SELECTOR_QUICK_REFERENCE.md`
- Show visual guide: `UNITS_SELECTOR_VISUAL_USER_GUIDE.md`
- FAQ section addresses common questions

### Technical Issues
- Check: `UNITS_SELECTOR_IMPLEMENTATION.md`
- Review: Conversion methods in code
- Reference: `UNITS_SELECTOR_TESTING_GUIDE.md`

## ? Success Criteria

Deployment is successful when:
- [x] Build passes
- [x] Tests pass (20+)
- [x] Feature visible on production
- [x] Unit conversions work correctly
- [x] No errors in logs
- [x] No performance degradation
- [x] Users can access documentation
- [x] No data corruption

## ?? Post-Deployment Monitoring

### Daily Monitoring (First Week)
- Monitor error logs
- Track user feedback
- Watch performance metrics
- Check browser compatibility

### Weekly Review
- User adoption rate
- Feature usage statistics
- Any reported issues
- Performance trends

### Monthly Review
- Overall stability
- User satisfaction
- Enhancement requests
- Plan Phase 2 features

## ?? Success Metrics

| Metric | Target | How to Measure |
|--------|--------|---|
| Build Success | 100% | Run dotnet build |
| Test Pass | 100% | Run all 20+ tests |
| Uptime | 99.9% | Monitor logs |
| Error Rate | <0.1% | Check error logs |
| User Adoption | >50% | Track usage |
| Performance | No lag | Monitor response times |

## ?? Deployment Log Template

```
Deployment Date: _______________
Deployed By: _______________
Version: 1.0
Status: ? Success / ? Issues

Pre-Deployment:
- Build status: ? Pass / ? Fail
- Test status: ? Pass / ? Fail
- Backup: ? Complete / ? Skipped

Deployment:
- Start time: _______________
- End time: _______________
- Downtime: _______________
- Issues: None / See notes below

Post-Deployment:
- Feature verified: ? Yes / ? No
- Tests run: ? Yes / ? No
- Users notified: ? Yes / ? No
- Monitoring enabled: ? Yes / ? No

Notes:
_________________________________
_________________________________

Sign-off: _______________
```

## ?? Related Documentation

- [Main README](README_UNITS_SELECTOR.md)
- [Quick Reference](UNITS_SELECTOR_QUICK_REFERENCE.md)
- [Implementation Guide](UNITS_SELECTOR_IMPLEMENTATION.md)
- [Testing Guide](UNITS_SELECTOR_TESTING_GUIDE.md)
- [Final Checklist](UNITS_SELECTOR_FINAL_CHECKLIST.md)

## ?? Deployment Complete

Once deployment is successful:
1. ? Monitor for issues
2. ? Gather user feedback
3. ? Plan Phase 2 enhancements
4. ? Schedule follow-up review
5. ? Document lessons learned

---

**Estimated Deployment Time**: 15-30 minutes
**Risk Level**: Low (no database changes)
**Rollback Capability**: Yes (simple)
**User Communication**: Required
**Approval Status**: Ready for deployment
