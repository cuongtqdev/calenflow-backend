document.addEventListener('DOMContentLoaded', () => {
    const path = window.location.pathname.toLowerCase();
    if (!path) {
        path = '/manager/dashboard';
    } else {
        console.log("curr ", path);
    }
    document.querySelectorAll(".nav-item").forEach(item => {
        if (path.includes(item.dataset.page)) {
            item.classList.add("highlighted");
        }
    });
});

document.querySelectorAll(".nav-item").forEach(el => {
    el.addEventListener("click", function () {
        const page = this.dataset.page;
        navigateTo(page);
    });
});

function navigateTo(page) {
    switch (page) {
        case 'dashboard':
            window.location.href = '/Manager/Dashboard';
            break;
        case 'calendar':
            window.location.href = '/Manager/Calendar';
            break;
        case 'candidate':
            window.location.href = '/Manager/Candidate';
            break;
        case 'reschedule':
            window.location.href = '/Manager/Reschedule';
            break;
        case 'information':
            window.location.href = '/Manager/Information';
            break;
    }
}

function logout() {
    window.location.href = '/Auth/Logout';
}
