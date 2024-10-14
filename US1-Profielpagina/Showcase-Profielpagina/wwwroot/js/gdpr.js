class GDPR {
    constructor() {
        this.showStatus();
        this.showContent();
        this.bindEvents();

        if (this.cookieStatus() !== 'accept') this.showGDPR();
    }

    bindEvents() {
        let buttonAccept = document.querySelector('.gdpr-consent__button--accept');
        let buttonReject = document.querySelector('.gdpr-consent__button--reject');

        buttonAccept.addEventListener('click', () => {
            this.cookieStatus('accept');
            this.showStatus();
            this.showContent();
            this.hideGDPR();
        });

        buttonReject.addEventListener('click', () => {
            this.cookieStatus('reject');
            this.showStatus();
            this.showContent();
            this.hideGDPR();
        });
    }

    showContent() {
        this.resetContent();
        const status = this.cookieStatus() == null ? 'not-chosen' : this.cookieStatus();
        const element = document.querySelector(`.content-gdpr-${status}`);
        if (element) {
            element.classList.add('show');
            element.classList.remove('hide');
        }
    }

    resetContent() {
        const classes = [
            '.content-gdpr-accept',
            '.content-gdpr-reject',
            '.content-gdpr-not-chosen'
        ];

        for (const c of classes) {
            const element = document.querySelector(c);
            if (element) {
                element.classList.add('hide');
                element.classList.remove('show');
            }
        }
    }

    showStatus() {
        const statusElement = document.getElementById('content-gpdr-consent-status')
        if (statusElement) {
            statusElement.innerHTML = this.cookieStatus() == null ? 'Niet gekozen' : this.cookieStatus();
        }
    }

    cookieStatus(status) {
        if (status) localStorage.setItem('gdpr-consent-choice', status);
        return localStorage.getItem('gdpr-consent-choice');
    }

    hideGDPR() {
        const gdprElement = document.querySelector('.gdpr-consent');
        if (gdprElement) {
            gdprElement.classList.add('hide');
            gdprElement.classList.remove('show');
        }
    }

    showGDPR() {
        const gdprElement = document.querySelector('.gdpr-consent');
        if (gdprElement) {
            gdprElement.classList.add('show');
            gdprElement.classList.remove('hide');
        }
    }
}

const gdpr = new GDPR();