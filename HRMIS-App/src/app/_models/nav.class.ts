import { navItems } from "../_nav";
import { User } from "./user.class";

export class Nav {
    public name: string;
    public url: string;
    public icon: string;
}
export class UserNav {
    private tempNav;
    constructor() {
        this.tempNav = [
            {
                name: 'DASHBAORD',
                url: '/dashboard',
                icon: 'fa fa-dashboard'
            },
            {
                name: 'DATABASE',
                url: '/database',
                icon: 'fa fa-database'
            },
            {
                name: 'HEALTH FACILITY',
                url: '/health-facility',
                icon: 'fa fa-hospital-o'
            },
            {
                name: 'EMPLOYEE PROFILE',
                url: '/profile',
                icon: 'fa fa-user-circle'
            },
            {
                name: 'DAILY WAGERS',
                url: '/dailywagerprofile',
                icon: 'fa fa-user-circle'
            },
            {
                name: 'VACANCY POSITION',
                url: '/vacancy-position',
                icon: 'fa fa-street-view'
            },
            {
                name: 'APPLICATION',
                url: '/application',
                icon: 'fa fa-file-text'
            },
            {
                name: 'ORDER / NOTIFICATION',
                url: '/order/0/1',
                icon: 'fa fa-file-text-o'
            },
            {
                name: 'REPORTING',
                url: '/reporting',
                icon: 'fa fa-table'
            },
            {
                name: 'USER',
                url: '/user',
                icon: 'fa fa-user-circle-o'
            }
        ];;
    }
    public getNav = (user: User): Nav[] => {
        if (user.UserName.toLowerCase().startsWith('ceo.')) {
            return this.makeNav(['DATABASE', 'REPORTING']);
        }
        //Administrator
        else if (user.RoleName == 'Administrator') {
            let hfmisCodeLength: number = user.HfmisCode.length;
            if (hfmisCodeLength === 0) {
                return this.makeNav([]);
            } else if (hfmisCodeLength === 1) {
                return this.makeNav(['DATABASE', 'ORDER / NOTIFICATION', 'USER']);
            } else if (hfmisCodeLength === 3) {
                return this.makeNav(['DATABASE', 'ORDER / NOTIFICATION']);
            }
            else if (hfmisCodeLength === 6 || hfmisCodeLength === 9 || hfmisCodeLength === 15 || hfmisCodeLength === 19) {
                return this.makeNav(['DATABASE', 'ORDER / NOTIFICATION', 'REPORTING']);
            }
        }
    }
    public makeNav = (links: string[]) => {
        links.forEach(link => {
            for (let index = 0; index < navItems.length; index++) {
                const element = navItems[index];
                if (element.name == link) {
                    this.tempNav.splice(index, 1);
                }
            }
        });
        return this.tempNav;
    }
}