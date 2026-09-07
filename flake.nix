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
        ];

        LD_LIBRARY_PATH = pkgs.lib.makeLibraryPath [
          #pkgs.raylib
          pkgs.glfw
          pkgs.libGL
          pkgs.xorg.libX11
          pkgs.xorg.libXrandr
          pkgs.xorg.libXi
          pkgs.xorg.libXcursor
          pkgs.xorg.libXinerama
        ];
      };
    };
}
