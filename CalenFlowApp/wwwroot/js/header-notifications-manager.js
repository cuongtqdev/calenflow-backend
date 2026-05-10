class QuickScheduleManager {
    constructor() {
        this.inviteBtn = document.getElementById('inviteBtn');
        this.invitePopup = document.getElementById('invitePopup');
        this.closePopupBtn = document.getElementById('closePopupBtn');
        this.inviteForm = document.getElementById('inviteForm');
        this.inviteEmailInput = document.getElementById('inviteEmail');
        this.modal = document.getElementById('quickScheduleModal');
        this.form = document.getElementById('quickScheduleForm');
        this.notifications = []
        this.unreadCount = 0
        this.init();
    }
    formatTimeAgo(date) {
        const diffMs = Date.now() - date.getTime();
        const diffSec = Math.floor(diffMs / 1000);
        const diffMin = Math.floor(diffSec / 60);
        const diffHour = Math.floor(diffMin / 60);
        const diffDay = Math.floor(diffHour / 24);

        if (diffDay > 0) return `${diffDay} day${diffDay > 1 ? "s" : ""} ago`;
        if (diffHour > 0) return `${diffHour} hour${diffHour > 1 ? "s" : ""} ago`;
        if (diffMin > 0) return `${diffMin} minute${diffMin > 1 ? "s" : ""} ago`;
        return "just now";
    }
    async loadUserEmail() {
        try {
            const response = await fetch('/Notification/GetUserEmail');
            if (response.ok) {
                const data = await response.json();
                return data.email;
            } else {
                console.error("Failed to load user email:", response.statusText);
            }
        } catch (error) {
            console.error("Error fetching user email:", error);
        }
        return null;
    }

    async loadConnection() {
        const userEmail = await this.loadUserEmail();
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl("/notificationHub")
            .build();

        this.connection.on("ReceiveNotification", async (message) => {
            const userEmail = await this.loadUserEmail();
            try {
                const response = await fetch('/Notification/GetNewNotificationManager', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                    },
                    body: JSON.stringify({ Email: userEmail, CandidateId: message.hiringId, Message: message.message, Type: message.type })
                })
                if (!response.ok) {
                    console.error("Failed to load notifications:", response.statusText);
                    return;
                }

                const result = await response.json();
                console.log("Notification fetch result:", result);
                if (result.status === "success") {
                    const notification = {
                        id: result.data.id,
                        title: result.data.title,
                        message: `${result.data.message}`,
                        time: this.formatTimeAgo(new Date(result.data.createdAt)),
                        type: result.data.type,
                        read: result.data.status, // giả sử status = true nghĩa là đã đọc
                        timestamp: new Date(result.data.createdAt)
                    };
                    this.addNotification(notification);
                }
            } catch (error) {
                console.error("Error fetching notifications:", error);
            }
        });
        console.log("SignalR connection initialized.");
        this.connection.start()
            .then(() => {
                console.log("Connected to SignalR hub!");
                console.log("user email:", userEmail);
                this.connection.invoke("JoinGroup", userEmail);
            })
            .catch(function (err) {
                console.error(err.toString());
            });
    }

    async init() {
        const today = new Date().toISOString().split('T')[0];
        document.getElementById('interviewDate').min = today;

        this.setupEventListeners();
        await this.loadNotifications()
        this.updateBadge()
        await this.loadConnection();
    }

    setupEventListeners() {
        const notificationBtn = document.getElementById("notificationBtn")
        const dropdown = document.getElementById("notificationDropdown")

        this.modal.addEventListener('click', (e) => {
            if (e.target === this.modal) {
                this.closeModal();
            }
        });

        document.addEventListener('keydown', (e) => {
            if (e.key === 'Escape' && this.modal.classList.contains('show')) {
                this.closeModal();
            }
        });

        this.form.addEventListener('input', () => {
            this.validateForm();
        });

        if (!notificationBtn || !dropdown) {
            console.warn("Notification elements not found")
            return
        }

        notificationBtn.addEventListener("click", (e) => {
            e.stopPropagation()
            this.toggleDropdown()
        })

        document.addEventListener("click", (e) => {
            if (!dropdown.contains(e.target) && !notificationBtn.contains(e.target)) {
                this.closeDropdown()
            }
        })

        dropdown.addEventListener("click", (e) => {
            const notificationItem = e.target.closest(".notification-item")
            if (notificationItem) {
                this.markAsRead(notificationItem.dataset.id)
            }
        })

        notificationBtn.addEventListener("keydown", (e) => {
            if (e.key === "Enter" || e.key === " ") {
                e.preventDefault()
                this.toggleDropdown()
            }
        })

        this.closePopupBtn.addEventListener('click', () => {
            this.invitePopup.classList.remove('show');
            this.inviteForm.reset();
        });
        window.addEventListener('click', (e) => {
            if (e.target === invitePopup) {
                invitePopup.classList.remove('show');
                inviteForm.reset();
            }
        });
        this.inviteForm.addEventListener('submit', (e) => {
            e.preventDefault();

            const email = this.inviteEmailInput.value;
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (email && !emailRegex.test(email)) {
                this.showToast('Error: Email không đúng định dạng.', 'error');
            } else {
                fetch('/Manager/InviteCandidate', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                    },
                    body: JSON.stringify({ Email: email })
                })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        // Gửi dữ liệu đi (mô phỏng)
                        console.log('Sending invitation...', email);
                        this.showToast('Send invite successfully', 'success');

                        // Đóng popup và reset form sau khi gửi thành công
                        invitePopup.classList.remove('show');
                        inviteForm.reset();
                    } else {
                        this.showToast(data.message, 'error');
                    }
                })
                .catch(err => {
                    console.error("Error updating status:", err);
                    this.showToast("Something went wrong!", "error");
                });
            }
        });
    }
    InviteForm() {
        this.invitePopup.classList.add('show');
    }

    openModal() {
        this.modal.classList.add('show');
        document.body.style.overflow = 'hidden';

        setTimeout(() => {
            document.getElementById('fullName').focus();
        }, 100);
    }

    closeModal() {
        this.modal.classList.remove('show');
        document.body.style.overflow = '';
    }

    validateForm() {
        const requiredFields = ['fullName', 'email', 'position', 'interviewType', 'interviewDate', 'interviewTime'];
        let isValid = true;

        requiredFields.forEach(fieldId => {
            const field = document.getElementById(fieldId);
            if (!field.value.trim()) {
                field.style.borderColor = '#ef4444';
                isValid = false;
            } else {
                field.style.borderColor = '#d1d5db';
            }
        });

        const email = document.getElementById('email').value;
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (email && !emailRegex.test(email)) {
            document.getElementById('email').style.borderColor = '#ef4444';
            isValid = false;
        }

        return isValid;
    }

    async scheduleInterview() {
        if (!this.validateForm()) {
            this.showToast('Please fill in all required fields correctly', 'error');
            return;
        }

        const formData = {
            fullName: document.getElementById('fullName').value,
            email: document.getElementById('email').value,
            position: document.getElementById('position').value,
            interviewType: document.getElementById('interviewType').value,
            date: document.getElementById('interviewDate').value,
            time: document.getElementById('interviewTime').value,
            duration: document.getElementById('duration').value,
            notes: document.getElementById('notes').value
        };

        const scheduleBtn = document.querySelector('.btn-primary');
        const originalText = scheduleBtn.innerHTML;
        scheduleBtn.innerHTML = 'Scheduling...';
        scheduleBtn.disabled = true;

        try {
            await this.simulateScheduling();

            this.showToast('Interview scheduled successfully!', 'success');
            this.closeModal();
            this.resetForm();

            console.log('Interview scheduled:', formData);

        } catch (error) {
            this.showToast('Failed to schedule interview. Please try again.', 'error');
        } finally {
            scheduleBtn.innerHTML = originalText;
            scheduleBtn.disabled = false;
        }
    }

    async simulateScheduling() {
        return new Promise((resolve) => {
            setTimeout(resolve, 2000);
        });
    }

    resetForm() {
        this.form.reset();
        document.getElementById('fullName').value = 'John Doe';
        document.getElementById('position').value = 'Senior Frontend Developer';

        const inputs = this.form.querySelectorAll('.form-input');
        inputs.forEach(input => {
            input.style.borderColor = '#d1d5db';
        });
    }

    showToast(message, type = 'success') {
        const toast = document.createElement('div');
        toast.style.cssText = `
                    position: fixed;
                    top: 20px;
                    right: 20px;
                    background: ${type === 'success' ? '#22c55e' : '#ef4444'};
                    color: white;
                    padding: 1rem 1.5rem;
                    border-radius: 0.5rem;
                    font-size: 0.9rem;
                    z-index: 10000;
                    opacity: 0;
                    transform: translateX(100%);
                    transition: all 0.3s ease;
                `;

        toast.textContent = message;
        document.body.appendChild(toast);

        setTimeout(() => {
            toast.style.opacity = '1';
            toast.style.transform = 'translateX(0)';
        }, 100);

        setTimeout(() => {
            toast.style.opacity = '0';
            toast.style.transform = 'translateX(100%)';
            setTimeout(() => {
                if (document.body.contains(toast)) {
                    document.body.removeChild(toast);
                }
            }, 300);
        }, 4000);
    }

    toggleDropdown() {
        const dropdown = document.getElementById("notificationDropdown")
        const isOpen = dropdown.classList.contains("show")

        if (isOpen) {
            this.closeDropdown()
        } else {
            this.openDropdown()
        }
    }

    openDropdown() {
        const dropdown = document.getElementById("notificationDropdown")
        dropdown.classList.add("show")

        const firstItem = dropdown.querySelector(".notification-item")
        if (firstItem) {
            firstItem.focus()
        }
    }

    closeDropdown() {
        const dropdown = document.getElementById("notificationDropdown")
        dropdown.classList.remove("show")
    }

    async loadNotifications() {
        const userEmail = await this.loadUserEmail();
        try {
            const response = await fetch('/Notification/GetHRNotification', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                },
                body: JSON.stringify({ Email: userEmail })
            })
            if (!response.ok) {
                console.error("Failed to load notifications:", response.statusText);
                return;
            }

            const result = await response.json();
            console.log("Notification fetch result:", result);
            if (result.status === "success") {
                console.log("Fetched notifications:", result.data);
                this.notifications = result.data.map((n) => {
                    console.log("rs: ", n);
                    return {
                        id: n.id,
                        title: n.title,
                        message: `${n.message}`,
                        time: this.formatTimeAgo(new Date(n.createdAt)),
                        type: n.type,
                        read: n.status,
                        timestamp: new Date(n.createdAt)
                    }
                });
                this.notifications.forEach(noti => {
                    this.renderNewNotification(noti);
                });
            }
        } catch (error) {
            console.error("Error fetching notifications:", error);
        }


        this.unreadCount = this.notifications.filter((n) => !n.read).length
        //this.notifications = [
        //    {
        //        id: "1",
        //        title: "Interview Reminder",
        //        message: "Frontend Developer interview with TechCorp Inc. tomorrow at 10:00 AM",
        //        time: "2 hours ago",
        //        type: "reminder",
        //        read: false,
        //        timestamp: new Date(Date.now() - 2 * 60 * 60 * 1000),
        //    },
        //    {
        //        id: "2",
        //        title: "Interview Confirmed",
        //        message: "Your Product Manager interview with StartupXYZ has been confirmed",
        //        time: "5 hours ago",
        //        type: "confirmation",
        //        read: false,
        //        timestamp: new Date(Date.now() - 5 * 60 * 60 * 1000),
        //    },
        //    {
        //        id: "3",
        //        title: "New Message",
        //        message: "HR from TechCorp sent you interview preparation materials",
        //        time: "1 day ago",
        //        type: "message",
        //        read: false,
        //        timestamp: new Date(Date.now() - 24 * 60 * 60 * 1000),
        //    },
        //    {
        //        id: "4",
        //        title: "Schedule Update",
        //        message: "Interview time changed from 2:00 PM to 3:00 PM",
        //        time: "2 days ago",
        //        type: "update",
        //        read: true,
        //        timestamp: new Date(Date.now() - 2 * 24 * 60 * 60 * 1000),
        //    },
        //]

    //    this.unreadCount = this.notifications.filter((n) => !n.read).length
    }

    markAsRead(notificationId) {
        const notification = this.notifications.find((n) => n.id === notificationId)
        if (notification && !notification.read) {
            notification.read = true

            const notificationElement = document.querySelector(`[data-id="${notificationId}"]`)
            if (notificationElement) {
                notificationElement.classList.remove("unread")
                notificationElement.classList.add("read")

                notificationElement.style.transition = "all 0.3s ease"
                notificationElement.style.opacity = "0.8"
                setTimeout(() => {
                    notificationElement.style.opacity = "1"
                }, 300)
            }

            this.unreadCount--
            this.updateBadge()

            this.dispatchNotificationEvent("notificationRead", { notificationId, notification })

            console.log(`Notification ${notificationId} marked as read`)
        }
    }

    markAllAsRead() {
        let markedCount = 0

        this.notifications.forEach((notification) => {
            if (!notification.read) {
                notification.read = true
                markedCount++

                const element = document.querySelector(`[data-id="${notification.id}"]`)
                if (element) {
                    element.classList.remove("unread")
                    element.classList.add("read")
                }
            }
        })

        this.unreadCount = 0
        this.updateBadge()

        if (markedCount > 0) {
            this.showToast(`${markedCount} notifications marked as read`)
        }

        this.dispatchNotificationEvent("allNotificationsRead", { markedCount })

        console.log(`${markedCount} notifications marked as read`)
    }

    updateBadge() {
        const badge = document.getElementById("notificationBadge")
        if (!badge) return

        if (this.unreadCount > 0) {
            badge.textContent = this.unreadCount > 99 ? "99+" : this.unreadCount
            badge.style.display = "flex"

            badge.classList.add("pulse")
            setTimeout(() => {
                badge.classList.remove("pulse")
            }, 2000)
        } else {
            badge.style.display = "none"
        }
    }

    addNotification(notification) {
        notification.id = notification.id || Date.now().toString()
        notification.read = false
        notification.timestamp = new Date()
        notification.time = "Just now"

        this.notifications.unshift(notification)
        this.unreadCount++
        this.updateBadge()

        this.renderNewNotification(notification)

        this.showToast(`New notification: ${notification.title}`)

        this.dispatchNotificationEvent("newNotification", { notification })

        console.log("New notification added:", notification)
    }

    renderNewNotification(notification) {
        const notificationList = document.querySelector(".notification-list")
        if (!notificationList) return

        const notificationHTML = this.createNotificationHTML(notification)
        notificationList.insertAdjacentHTML("afterbegin", notificationHTML)

        const newElement = notificationList.firstElementChild
        newElement.style.opacity = "0"
        newElement.style.transform = "translateY(-10px)"

        setTimeout(() => {
            newElement.style.transition = "all 0.3s ease"
            newElement.style.opacity = "1"
            newElement.style.transform = "translateY(0)"
        }, 100)
    }

    createNotificationHTML(notification) {
        const iconMap = {
            reminder: `<svg viewBox="0 0 24 24" fill="none">
                <rect x="3" y="4" width="18" height="18" rx="2" ry="2" stroke="currentColor" stroke-width="2"/>
                <line x1="16" y1="2" x2="16" y2="6" stroke="currentColor" stroke-width="2"/>
                <line x1="8" y1="2" x2="8" y2="6" stroke="currentColor" stroke-width="2"/>
                <line x1="3" y1="10" x2="21" y2="10" stroke="currentColor" stroke-width="2"/>
            </svg>`,
            confirmation: `<svg viewBox="0 0 24 24" fill="none">
                <path d="M22 11.08V12A10 10 0 1 1 5.93 7.25" stroke="currentColor" stroke-width="2"/>
                <polyline points="22,4 12,14.01 9,11.01" stroke="currentColor" stroke-width="2"/>
            </svg>`,
            message: `<svg viewBox="0 0 24 24" fill="none">
                <path d="M4 4H20C21.1 4 22 4.9 22 6V18C22 19.1 21.1 20 20 20H4C2.9 20 2 19.1 2 18V6C2 4.9 2.9 4 4 4Z" stroke="currentColor" stroke-width="2"/>
                <polyline points="22,6 12,13 2,6" stroke="currentColor" stroke-width="2"/>
            </svg>`,
            update: `<svg viewBox="0 0 24 24" fill="none">
                <circle cx="12" cy="12" r="10" stroke="currentColor" stroke-width="2"/>
                <path d="M12 6V12L16 14" stroke="currentColor" stroke-width="2"/>
            </svg>`,
        }

        return `
            <div class="notification-item ${notification.read ? "read" : "unread"}" data-id="${notification.id}">
                <div class="notification-icon-wrapper">
                    ${iconMap[notification.type] || iconMap.message}
                </div>
                <div class="notification-content">
                    <div class="notification-title">${notification.title}</div>
                    <div class="notification-message">${notification.message}</div>
                    <div class="notification-time">${notification.time}</div>
                </div>
                ${!notification.read ? '<div class="notification-dot"></div>' : ""}
            </div>
        `
    }
    dispatchNotificationEvent(eventName, data) {
        const event = new CustomEvent(eventName, {
            detail: data,
            bubbles: true,
        })
        document.dispatchEvent(event)
    }

    getNotifications() {
        return this.notifications
    }

    getUnreadCount() {
        return this.unreadCount
    }

    clearAllNotifications() {
        this.notifications = []
        this.unreadCount = 0
        this.updateBadge()

        const notificationList = document.querySelector(".notification-list")
        if (notificationList) {
            notificationList.innerHTML = '<div class="no-notifications">No notifications</div>'
        }
    }

    simulateNewNotification() {
        const sampleNotifications = [
            {
                title: "Interview Scheduled",
                message: "New interview scheduled for next week",
                type: "confirmation",
            },
            {
                title: "Reminder",
                message: "Don't forget your interview tomorrow",
                type: "reminder",
            },
            {
                title: "Message Received",
                message: "You have a new message from HR",
                type: "message",
            },
        ]

        const randomNotification = sampleNotifications[Math.floor(Math.random() * sampleNotifications.length)]
        this.addNotification(randomNotification)
    }
}

function openQuickScheduleModal() {
    if (window.quickScheduleManager) {
        window.quickScheduleManager.openModal();
    }
}

function closeQuickScheduleModal() {
    if (window.quickScheduleManager) {
        window.quickScheduleManager.closeModal();
    }
}

function scheduleInterview() {
    if (window.quickScheduleManager) {
        window.quickScheduleManager.scheduleInterview();
    }
}

function markAllAsRead() {
    if (window.quickScheduleManager) {
        window.quickScheduleManager.markAllAsRead()
    }
}
function OpenInviteForm() {
    if (window.quickScheduleManager) {
        window.quickScheduleManager.InviteForm();
    }
}
function viewAllNotifications() {
    console.log("View all notifications clicked")
    if (window.notificationSystem) {
        const notifications = window.notificationSystem.getNotifications()
        console.log("All notifications:", notifications)
    }

    alert("This would navigate to the full notifications page")
}


document.addEventListener('DOMContentLoaded', () => {
    window.quickScheduleManager = new QuickScheduleManager();
});

document.addEventListener("notificationRead", (e) => {
    console.log("Notification read event:", e.detail)
})

document.addEventListener("allNotificationsRead", (e) => {
    console.log("All notifications read event:", e.detail)
})

document.addEventListener("newNotification", (e) => {
    console.log("New notification event:", e.detail)
})

if (typeof module !== "undefined" && module.exports) {
    module.exports = NotificationSystem
}