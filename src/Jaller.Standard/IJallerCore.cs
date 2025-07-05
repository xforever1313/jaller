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

using Jaller.Standard.Bulk;
using Jaller.Standard.Configuration;
using Jaller.Standard.FileManagement;
using Jaller.Standard.FolderManagement;
using Jaller.Standard.Ipfs;
using Jaller.Standard.Logging;
using Jaller.Standard.Monitoring;
using Jaller.Standard.Search;
using Jaller.Standard.UserManagement;

namespace Jaller.Standard;

public interface IJallerCore
{
    // ---------------- Properties ----------------

    IJallerBulkOperations BulkOperations { get; }

    IJallerCanaryFileMonitor CanaryFileMonitor { get; }

    IJallerConfig Config { get; }

    IJallerFolderManager Folders { get; }

    IJallerFileManager Files { get; }

    IJallerIpfsManager Ipfs { get; }

    IJallerLogger Log { get; }

    IJallerSearch Search { get; }

    IUserManager UserManager { get; }
}
