using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jsonDbUnitTests
{
    class Poco1 : jsonDb.JDbObject
    {
       public string LastName;
       public string FirstName; 
       public DateTime BirthDate;
       public bool USCitizen;
       public double Height;
       public string SSN;
    }

    class BaseObject {

        public Guid Guid;
        public DateTime CreationDate, LastModified;
        public string Domain;
        public string Name;

        public virtual void Create() {

            this.Guid = Guid.NewGuid();
            this.CreationDate = this.LastModified = DateTime.Now;
        }
    }

    class User : BaseObject {

        public Groups Groups;
        
        public User Create(string domain, string name) {
            base.Create();
            this.Domain = domain;
            this.Name = name;
            return this;
        }

        public User AddGroup(Group group) {
            if(this.Groups == null)
                this.Groups = new Groups();
            this.Groups.Add(group);
            return this;
        }

        public User AddGroups(List<Group> groups) {
            if(this.Groups == null)
                this.Groups = new Groups();
            this.Groups.AddRange(groups);
            return this;
        }

        public bool IsAuthorized(Roles roles) {

            foreach(var r in roles)
                if(!this.IsAuthorized(r))
                    return false;
            return true;
        }

        public bool IsAuthorized(Role role) {
            foreach(var g in this.Groups)
                foreach(var r in g.Roles)
                    if(r.Name == role.Name)
                        return true;
            return false;
        }
    }
    
    class Users : List<User> {

        public User GetByName(string name) {

            return this.Where(r => r.Name == name).FirstOrDefault();
        }
    }
    
    class Role : BaseObject {

        public string Name;

        public Role(string name) {
            this.Name = name;
            base.Create();
        }
    }

    class Roles : List<Role> {

        public void Add(List<string> roles) {

            foreach(var role in roles)
                this.Add(new Role(role));
        }
        public Role GetByName(string name) {

            return this.Where(r => r.Name == name).FirstOrDefault();
        }
    }

    class Group : BaseObject {

        public string Name;
        public Roles Roles = new Roles();

        public Group()
        {
 	        base.Create();
        }
    }

    class Groups : List<Group> {

        public Group GetByName(string name) {

            return this.Where(r => r.Name == name).FirstOrDefault();
        }
    }

    class AuthorizationSystem : jsonDb.JDbObject {
        
        public Users  Users        = new Users();   
        public Groups Groups       = new Groups();
        public Roles VMRoles       = new Roles();
        public Roles TemplateRoles = new Roles();
        public Roles UserRoles     = new Roles();

        public Roles GetAllRoles() {

            var roles = new Roles();
            roles.AddRange(this.VMRoles);
            roles.AddRange(this.TemplateRoles);
            roles.AddRange(this.UserRoles);
            return roles;
        }
    };
}
