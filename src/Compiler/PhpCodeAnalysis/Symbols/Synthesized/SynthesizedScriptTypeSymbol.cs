﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslyn.Utilities;

namespace Pchp.CodeAnalysis.Symbols
{
    /// <summary>
    /// internal static class &lt;Script&gt; { ... }
    /// </summary>
    class SynthesizedScriptTypeSymbol : NamedTypeSymbol
    {
        readonly PhpCompilation _compilation;

        /// <summary>
        /// Optional. Real assembly entry point method.
        /// </summary>
        internal MethodSymbol EntryPointSymbol { get; set; }

        /// <summary>
        /// Method that enumerates all referenced global functions.
        /// 
        /// EnumerateReferencedFunctions(Action&lt;string, RuntimeMethodHandle&gt; callback)
        /// </summary>
        internal MethodSymbol EnumerateReferencedFunctionsSymbol => _enumerateReferencedFunctionsSymbol ?? (_enumerateReferencedFunctionsSymbol = CreateEnumerateReferencedFunctionsSymbol());
        MethodSymbol _enumerateReferencedFunctionsSymbol;

        public SynthesizedScriptTypeSymbol(PhpCompilation compilation)
        {
            _compilation = compilation;
        }

        public override int Arity => 0;

        public override Symbol ContainingSymbol => _compilation.SourceModule;

        internal override IModuleSymbol ContainingModule => _compilation.SourceModule;

        public override Accessibility DeclaredAccessibility => Accessibility.Internal;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override bool IsAbstract => false;

        public override bool IsSealed => false;

        public override bool IsStatic => true;

        public override ImmutableArray<Location> Locations
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override string Name => $"<{WellKnownMemberNames.DefaultScriptClassName}>";

        public override NamedTypeSymbol BaseType => _compilation.CoreTypes.Object;

        public override TypeKind TypeKind => TypeKind.Class;

        internal override bool IsInterface => false;

        internal override bool IsWindowsRuntimeImport => false;

        internal override TypeLayout Layout => default(TypeLayout);

        internal override bool MangleName => false;

        internal override ObsoleteAttributeData ObsoleteAttributeData => null;

        internal override bool ShouldAddWinRTMembers => false;

        public override ImmutableArray<Symbol> GetMembers()
        {
            var list = new List<Symbol>()
            {
                this.EnumerateReferencedFunctionsSymbol,
            };

            //
            if (EntryPointSymbol != null)
                list.Add(EntryPointSymbol);

            //
            return list.AsImmutable();
        }

        public override ImmutableArray<Symbol> GetMembers(string name) => GetMembers().Where(m => m.Name == name).AsImmutable();

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers() => ImmutableArray<NamedTypeSymbol>.Empty;

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name) => ImmutableArray<NamedTypeSymbol>.Empty;

        internal override ImmutableArray<NamedTypeSymbol> GetDeclaredInterfaces(ConsList<Symbol> basesBeingResolved) => ImmutableArray<NamedTypeSymbol>.Empty;

        internal override IEnumerable<IFieldSymbol> GetFieldsToEmit() => ImmutableArray<IFieldSymbol>.Empty;

        internal override ImmutableArray<NamedTypeSymbol> GetInterfacesToEmit() => ImmutableArray<NamedTypeSymbol>.Empty;

        /// <summary>
        /// Method that enumerates all referenced global functions.
        /// EnumerateReferencedFunctions(Action&lt;string, RuntimeMethodHandle&gt; callback)
        /// </summary>
        MethodSymbol CreateEnumerateReferencedFunctionsSymbol()
        {
            var compilation = DeclaringCompilation;
            var action_T2 = compilation.GetWellKnownType(WellKnownType.System_Action_T2);
            var action_string_method = action_T2.Construct(compilation.CoreTypes.String, compilation.CoreTypes.RuntimeMethodHandle);

            var method = new SynthesizedMethodSymbol(this, "EnumerateReferencedFunctions", true, compilation.CoreTypes.Void, Accessibility.Public);
            method.SetParameters(new SynthesizedParameterSymbol(method, action_string_method, 0, RefKind.None, "callback"));

            //
            return method;
        }
    }
}
