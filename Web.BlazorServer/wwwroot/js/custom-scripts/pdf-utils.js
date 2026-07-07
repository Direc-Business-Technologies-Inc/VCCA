window.openPdfNewTab = function (base64) {
    const bytes = Uint8Array.from(atob(base64), c => c.charCodeAt(0));
    const blob = new Blob([bytes], { type: 'application/pdf' });
    const url = URL.createObjectURL(blob);
    const tab = window.open(url, '_blank');
    if (tab) tab.focus();
};
