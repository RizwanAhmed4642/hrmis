export class Notification {
    constructor(type: string, content?: string, icon?: string) {
        this.show = true;
        this.type = 'bg-';
        this.icon = 'fa-';
        switch (type) {
            case 'success':
                this.icon += icon ? icon : 'check';
                this.content = content ? content : 'Successfully saved...'
                this.type += type;
                break;
            case 'danger':
                this.icon += icon ? icon : 'times';
                this.content = content ? content : 'Something went wrong...'
                this.type += type;
                break;
            case 'warning':
                this.icon += icon ? icon : 'exclamation';
                this.content = content ? content : 'Warning!'
                this.type += type;
                break;
            default:
                break;
        }
    }
    public show: boolean;
    public icon: string;
    public content: string;
    public type: string;
}