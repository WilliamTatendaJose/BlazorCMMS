// Document Viewer JavaScript Helpers

window.downloadFile = function (url, fileName) {
    const link = document.createElement('a');
    link.href = url;
    link.download = fileName;
    link.target = '_blank';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
};

window.openInNewTab = function (url) {
    window.open(url, '_blank');
};

window.printDocument = function (url) {
    const printWindow = window.open(url, '_blank');
    if (printWindow) {
        printWindow.onload = function () {
            printWindow.print();
        };
    }
};

// PDF.js integration for advanced PDF viewing (optional)
window.initPdfViewer = async function (containerId, pdfUrl) {
    // This would integrate with PDF.js library if included
    console.log('PDF Viewer initialized for:', pdfUrl);
};

// Image pan and zoom functionality
window.initImagePanZoom = function (containerId) {
    const container = document.getElementById(containerId);
    if (!container) return;

    let isDragging = false;
    let startX, startY, scrollLeft, scrollTop;

    container.addEventListener('mousedown', (e) => {
        isDragging = true;
        container.style.cursor = 'grabbing';
        startX = e.pageX - container.offsetLeft;
        startY = e.pageY - container.offsetTop;
        scrollLeft = container.scrollLeft;
        scrollTop = container.scrollTop;
    });

    container.addEventListener('mouseleave', () => {
        isDragging = false;
        container.style.cursor = 'default';
    });

    container.addEventListener('mouseup', () => {
        isDragging = false;
        container.style.cursor = 'default';
    });

    container.addEventListener('mousemove', (e) => {
        if (!isDragging) return;
        e.preventDefault();
        const x = e.pageX - container.offsetLeft;
        const y = e.pageY - container.offsetTop;
        const walkX = (x - startX) * 2;
        const walkY = (y - startY) * 2;
        container.scrollLeft = scrollLeft - walkX;
        container.scrollTop = scrollTop - walkY;
    });

    // Mouse wheel zoom
    container.addEventListener('wheel', (e) => {
        if (e.ctrlKey) {
            e.preventDefault();
            // Zoom logic would be handled by Blazor component
        }
    });
};

// Fullscreen API helper
window.toggleFullscreen = function (elementId) {
    const element = document.getElementById(elementId);
    if (!element) return;

    if (!document.fullscreenElement) {
        element.requestFullscreen().catch(err => {
            console.error(`Error attempting to enable fullscreen: ${err.message}`);
        });
    } else {
        document.exitFullscreen();
    }
};

// Copy to clipboard
window.copyToClipboard = function (text) {
    navigator.clipboard.writeText(text).then(() => {
        console.log('Copied to clipboard');
    }).catch(err => {
        console.error('Failed to copy:', err);
    });
};
