using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceLib.CustomExceptions
{
    public class AlreadyADefaultLanguage : Exception
    {
        public AlreadyADefaultLanguage() : base("") { }
    }

    public class DatabaseQueryError : Exception
    {
        public DatabaseQueryError() : base("") { }
    }

    public class SaveFailed : Exception
    {
        public SaveFailed() : base("") { }
    }

    public class UsernameTaken : Exception
    {
        public UsernameTaken() : base("") { }
    }

    public class UsernameNotUnique:Exception
    {
        public UsernameNotUnique() : base("") { }
    }

    public class ImagePathToSaveNotFound : Exception
    {
        public ImagePathToSaveNotFound() : base("") { }
    }
}
