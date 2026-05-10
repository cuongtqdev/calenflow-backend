class NotificationSystem {
    constructor() {
        this.connection = null;
        this.notifications = []
        this.unreadCount = 0
        this.init()
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
                const response = await fetch('/Notification/GetNewNotification', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                    },
                    body: JSON.stringify({ Email: userEmail, HiringId: message.hiringId, Message: message.message, Type: message.type })
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
                        message: `${result.data.message} (Company ${result.data.companyName})`,
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
        this.setupEventListeners()
        await this.loadNotifications()
        this.updateBadge()
        await this.loadConnection()
    }

    setupEventListeners() {
        const notificationBtn = document.getElementById("notificationBtn")
        const dropdown = document.getElementById("notificationDropdown")

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

    async loadNotifications() {
        console.log("load noti");
        const userEmail = await this.loadUserEmail();
        try {
            const response = await fetch('/Notification/GetUserNotification', {
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
                        message: `${n.message} (Company ${n.companyName})`, 
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
    }

    async markAsRead(notificationId) {
        const notification = this.notifications.find((n) => {
            console.log("Checking notification:", n.id, "against", notificationId);
            return Number(n.id) === Number(notificationId);
        })
        console.log("Marking notification as read:", notification);
        if (notification && !notification.read) {
            try {
                const response = await fetch(`/Notification/MarkAsRead?id=${Number(notificationId)}`);

                if (!response.ok) {
                    console.error("Failed to load notifications:", response.statusText);
                    return;
                }

                const result = await response.json();
                if (result.status === "success") {
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
                }
            } catch (error) {
                console.error("Fetch error:", error);
            }
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

    showToast(message) {
        const toast = document.createElement("div")
        toast.className = "notification-toast"
        toast.textContent = message
        toast.style.cssText = `
            position: fixed;
            top: 20px;
            right: 20px;
            background: #1e293b;
            color: white;
            padding: 0.75rem 1rem;
            border-radius: 0.5rem;
            font-size: 0.9rem;
            z-index: 10000;
            opacity: 0;
            transform: translateX(100%);
            transition: all 0.3s ease;
            max-width: 300px;
        `

        document.body.appendChild(toast)

        setTimeout(() => {
            toast.style.opacity = "1"
            toast.style.transform = "translateX(0)"
        }, 100)

        setTimeout(() => {
            toast.style.opacity = "0"
            toast.style.transform = "translateX(100%)"
            setTimeout(() => {
                document.body.removeChild(toast)
            }, 300)
        }, 3000)
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

function markAllAsRead() {
    if (window.notificationSystem) {
        window.notificationSystem.markAllAsRead()
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

document.addEventListener("DOMContentLoaded", () => {
    window.notificationSystem = new NotificationSystem()

})

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
