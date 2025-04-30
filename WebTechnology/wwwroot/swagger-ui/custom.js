window.onload = function() {
    // Add logo to the topbar
    const topbar = document.querySelector('.topbar');
    if (topbar) {
        const logo = document.createElement('img');
        logo.src = '/images/logo.png';
        logo.style.height = '40px';
        logo.style.margin = '10px';
        topbar.insertBefore(logo, topbar.firstChild);
    }
    
    // Add custom footer
    const footer = document.createElement('div');
    footer.style.textAlign = 'center';
    footer.style.padding = '20px';
    footer.style.marginTop = '20px';
    footer.style.borderTop = '1px solid #ddd';
    footer.innerHTML = '© 2023 Hien v Tam API. All rights reserved.';
    document.querySelector('.swagger-ui').appendChild(footer);
}; 