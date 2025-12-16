// Download helper function for file exports
window.downloadFile = function(data, filename, mimeType) {
    const blob = new Blob([data], { type: mimeType });
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = filename;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    window.URL.revokeObjectURL(url);
};

// Export functionality module
window.exportModule = {
    // Initialize export module
    init: function() {
        console.log('Export module initialized');
    },

    // Show export progress
    showProgress: function(message) {
        console.log('Export progress:', message);
    },

    // Format file size for display
    formatFileSize: function(bytes) {
        if (bytes === 0) return '0 Bytes';
        const k = 1024;
        const sizes = ['Bytes', 'KB', 'MB', 'GB'];
        const i = Math.floor(Math.log(bytes) / Math.log(k));
        return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i];
    },

    // Check browser support
    checkSupport: function() {
        return {
            csv: true,
            json: true,
            excel: true,
            download: !!window.URL && !!document.createElement('a').download
        };
    }
};
