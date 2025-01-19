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

using System.Reflection;
using System.Text;
using Jaller.Core.Exceptions;
using Jaller.Standard.Configuration;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace Jaller.Core.Configuration
{
    internal sealed class ConfigCompiler
    {
        // ---------------- Fields ----------------

        private readonly Func<string> readConfigFile;

        // ---------------- Constructor ----------------

        public ConfigCompiler( FileInfo configFile ) :
            this( () => File.ReadAllText( configFile.FullName ) )
        {
        }

        public ConfigCompiler( string code ) :
            this( () => code )
        {
        }

        private ConfigCompiler( Func<string> readConfigFile )
        {
            this.readConfigFile = readConfigFile;
        }

        // ---------------- Properties ----------------

        internal string? ConfigFileSourceCode { get; private set; }

        // ---------------- Methods ----------------

        public void Preprocess()
        {
            // Nothing special to preprocess for Jaller,
            // just read the source code and we're good to go!
            this.ConfigFileSourceCode = this.readConfigFile();
        }

        public IJallerConfig Compile()
        {
            if( this.ConfigFileSourceCode is null )
            {
                throw new InvalidOperationException(
                    "Can not compile config file, it has not been preprocessed yet."
                );
            }

            const string expectedType = "Jaller.Config.JallerUserConfig";

            Assembly asm = CompileAsm();

            Type? type = asm.GetType( expectedType );
            if( type is null )
            {
                throw new ConfigCompilerException(
                    $"Type of {expectedType} not found in dynamic assembly."
                );
            }

            object? obj = Activator.CreateInstance( type );
            if( obj is null )
            {
                throw new ConfigCompilerException(
                    "Failed to activate compiled config object."
                );
            }

            return (IJallerConfig)obj;
        }

        private Assembly CompileAsm()
        {
            string code = GetCode();

            // Taken from https://stackoverflow.com/a/47732064
            Assembly[] refs = AppDomain.CurrentDomain.GetAssemblies();
            List<MetadataReference> references = refs.Where( a => a.IsDynamic == false )
                .Select( a => MetadataReference.CreateFromFile( a.Location ) )
                .ToList<MetadataReference>();

            references.Add( MetadataReference.CreateFromFile( typeof( Uri ).Assembly.Location ) );

            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText( code );

            var options = new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary
            )
            {
            };

            CSharpCompilation compilation = CSharpCompilation.Create(
                "ConfigurationAssembly",
                syntaxTrees: [syntaxTree],
                references: references,
                options: options
            );

            using var ms = new MemoryStream();
            EmitResult result = compilation.Emit( ms );

            if( result.Success == false )
            {
                var errors = new List<string>();
                foreach( Diagnostic diagnostic in result.Diagnostics )
                {
                    errors.Add( $"{diagnostic.Id}: {diagnostic.GetMessage()}" );
                }

                throw new InvalidConfigurationException(
                    "Errors compiling config file.  Please fix these errors and try again.",
                    errors
                );
            }

            ms.Seek( 0, SeekOrigin.Begin );
            return Assembly.Load( ms.ToArray() );
        }

        private string GetCode()
        {
            if( this.ConfigFileSourceCode is null )
            {
                throw new InvalidOperationException(
                    "Can not compile config file, it has not been preprocessed yet."
                );
            }

            var namespaceBuilder = new StringBuilder();
            namespaceBuilder.AppendLine( "using System;" );
            namespaceBuilder.AppendLine( "using System.Collections.Generic;" );
            namespaceBuilder.AppendLine( "using System.IO;" );
            namespaceBuilder.AppendLine( "using Jaller.Core;" );
            namespaceBuilder.AppendLine( "using Jaller.Core.Configuration;" );
            namespaceBuilder.AppendLine( "using Jaller.Core.Exceptions;" );
            namespaceBuilder.AppendLine( "using Jaller.Standard;" );
            namespaceBuilder.AppendLine( "using Jaller.Standard.Configuration;" );
            namespaceBuilder.AppendLine( "using Jaller.Standard.Logging;" );

            string code = this.ConfigFileSourceCode;

            code =
$@"{namespaceBuilder}
namespace Jaller.Config
{{
    public sealed class JallerUserConfig : JallerConfig
    {{
        public JallerUserConfig() :
            base()
        {{
            {code}
        }}
    }}
}}
";
            return code;
        }
    }
}
