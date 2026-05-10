class CalendflowSidebar {
    constructor() {
        this.currentRole = 'user';
        this.init();
    }

    init() {
        this.setupEventListeners();
        this.showRoleNotification();
    }

    setupEventListeners() {

        document.querySelectorAll('.interview-card').forEach(card => {
            card.addEventListener('click', (e) => {
                const interviewId = card.dataset.id;
                this.openInterviewDetails(interviewId);
            });
        });
    }

    switchRole(newRole) {
        document.querySelector('.role-text').textContent = newRole;
        this.currentRole = newRole.toLowerCase().replace('/', '');

        this.showRoleNotification(newRole);

        this.updateUIForRole(newRole);

        console.log(`Switched to ${newRole} role`);
    }

    showRoleNotification(role = 'User/Guest') {
        const notification = document.getElementById('roleNotification');
        if (notification) {
            notification.textContent = `Switched to ${role} role`;
            notification.style.display = 'flex';

            setTimeout(() => {
                notification.style.display = 'none';
            }, 5000);
        }
    }

    updateUIForRole(role) {
        const pageTitle = document.querySelector('.page-title h1');
        const pageSubtitle = document.querySelector('.page-subtitle');

        if (pageTitle && pageSubtitle) {
            switch (role) {
                case 'Interviewer':
                    pageTitle.textContent = 'Interview Management';
                    pageSubtitle.textContent = 'Manage your interview schedule and candidates';
                    break;
                case 'Admin':
                    pageTitle.textContent = 'Admin Dashboard';
                    pageSubtitle.textContent = 'Manage users, interviews, and system settings';
                    break;
                default:
                    pageTitle.textContent = 'My Interviews';
                    pageSubtitle.textContent = 'Track your interview schedule and requests';
            }
        }
    }


    openInterviewDetails(interviewId) {
        const interviews = {
            '1': {
                title: 'Frontend Developer Interview',
                company: 'TechCorp Inc.',
                interviewer: 'John Smith, Senior Developer',
                date: '2025-01-19',
                time: '10:00 AM',
                duration: '45 min',
                status: 'confirmed'
            },
            '2': {
                title: 'Product Manager Interview',
                company: 'StartupXYZ',
                interviewer: 'Sarah Davis, Product Lead',
                date: '2025-01-22',
                time: '2:00 PM',
                duration: '60 min',
                status: 'pending'
            }
        };

        const interview = interviews[interviewId];
        if (interview) {
            alert(`Interview Details:\n\nTitle: ${interview.title}\nCompany: ${interview.company}\nInterviewer: ${interview.interviewer}\nDate: ${interview.date}\nTime: ${interview.time}\nDuration: ${interview.duration}\nStatus: ${interview.status}`);
        }
    }

    getCurrentRole() {
        return this.currentRole;
    }

    setRole(role) {
        this.switchRole(role);
    }
}

document.addEventListener('DOMContentLoaded', () => {
    const path = window.location.pathname.toLowerCase();
    if (!path) {
        path = '/user/dashboard';
    } else {
        console.log("curr ", path);
    }
    document.querySelectorAll(".nav-item").forEach(item => {
        if (path.includes(item.dataset.page)) {
            item.classList.add("highlighted");
        }
    });
    window.calendflowSidebar = new CalendflowSidebar();
});

document.querySelectorAll(".nav-item").forEach(el => {
    el.addEventListener("click", function () {
        const page = this.dataset.page;
        navigateTo(page);
    });
});
function ScanCV() {
    window.location.href = '/User/ScanCV';
}
function navigateTo(page) {
    switch (page) {
        case 'dashboard':
            window.location.href = '/User/Dashboard';
            break;
        case 'request':
            window.location.href = '/User/RequestMeeting';
            break;
        case 'reschedule':
            window.location.href = '/User/Reschedule';
            break;
        case 'information':
            window.location.href = '/User/Information';
            break;
    }
}

function logout() {
    window.location.href = '/Auth/Logout';
}
