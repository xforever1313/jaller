//
// Jaller - An advanced IPFS Gateway
// Copyright (C) 2025 Seth Hendrick
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.
//

using Jaller.Server.Models;
using Jaller.Standard;

namespace Jaller.Server.Pages.Bit;

public sealed class IndexModel : BasePageModel
{
    // ---------------- Fields ----------------

    private readonly IJallerCore core;

    // ---------------- Constructor ----------------

    public IndexModel( IJallerCore core ) :
        base( core )
    {
        this.core = core;
    }

    // ---------------- Properties ----------------

    public UserRolesModel? UserRoles { get; private set; }

    public bool PublicRegistrationEnabled => this.core.Config.Users.AllowPublicRegistration;

    public IReadOnlyCollection<string>? ApprovedAdminHosts => this.core.Config.Web.AllowedAdminHosts;

    // ---------------- Methods ----------------

    public void OnGet()
    {
        this.UserRoles = this.User.ToRolesModel( this.core, this.Request );
    }
}
