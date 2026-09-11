{
  description = "";

  inputs.nixpkgs.url = "github:NixOS/nixpkgs/nixos-unstable";

  outputs = { self, nixpkgs }:
    let
      system = "x86_64-linux";
      pkgs = import nixpkgs { inherit system; };
    in {
      devShells.${system}.default = pkgs.mkShell {
        packages = with pkgs; [
          dotnetCorePackages.sdk_10_0

          gtk3
          glib
          gsettings-desktop-schemas
        ];

        shellHook = ''
          export XDG_DATA_DIRS="${pkgs.gsettings-desktop-schemas}/share/gsettings-schemas/${pkgs.gsettings-desktop-schemas.name}:${pkgs.gtk3}/share/gsettings-schemas/${pkgs.gtk3.name}:''${XDG_DATA_DIRS:-}" 
        '';

        LD_LIBRARY_PATH = pkgs.lib.makeLibraryPath [
          pkgs.glfw
          pkgs.libGL
          pkgs.xorg.libX11
          pkgs.xorg.libXrandr
          pkgs.xorg.libXi
          pkgs.xorg.libXcursor
          pkgs.xorg.libXinerama
          pkgs.gtk3
          pkgs.glib
        ];
      };
    };
}
