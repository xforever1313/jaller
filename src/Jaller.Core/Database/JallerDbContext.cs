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

using Jaller.Core.Database.Engines;
using Jaller.Core.Database.Models;
using Jaller.Standard;
using Microsoft.EntityFrameworkCore;

namespace Jaller.Core.Database;

internal sealed class JallerDbContext : DbContext
{
    // ---------------- Fields ----------------

    private readonly IJallerCore core;

    private readonly IDatabaseEngine databaseEngine;

    // ---------------- Constructor ----------------

    public JallerDbContext( IJallerCore core ) :
        this(
            core,
            new SqliteDatabaseEngine(
                core.Config.Database.SqliteDatabaseLocation,
                core.Config.Database.SqlitePool
            )
        )
    {
    }

    public JallerDbContext( IJallerCore core, IDatabaseEngine databaseEngine )
    {
        this.core = core;
        this.databaseEngine = databaseEngine;
    }

    // ---------------- Properties ----------------

    public DbSet<ContentId> Cids { get; set; }

    public DbSet<Models.File> Files { get; set; }

    public DbSet<FileDownloadInformation> FileDownloadInformation { get; set; }

    public DbSet<FileUploadInformation> FileUploadInformation { get; set; }

    public DbSet<Folder> Folders { get; set; }

    public DbSet<PasswordAuthentication> Passwords { get; set; }

    public DbSet<User> Users { get; set; }

    // ---------------- Methods ----------------

    public void EnsureCreated()
    {
        this.core.Log.Information( "Creating Database" );
        this.Database.EnsureCreated();
    }

    protected override void OnConfiguring( DbContextOptionsBuilder optionsBuilder )
    {
        this.databaseEngine.OnConfiguring( optionsBuilder );
        base.OnConfiguring( optionsBuilder );
    }

    protected override void OnModelCreating( ModelBuilder modelBuilder )
    {
        this.databaseEngine.OnModelCreating( modelBuilder );

        Folder.OnModelCreating( modelBuilder );
        ContentId.OnModelCreating( modelBuilder );
        Models.File.OnModelCreating( modelBuilder );
        PasswordAuthentication.OnModelCreating( modelBuilder );

        base.OnModelCreating( modelBuilder );
    }
}
