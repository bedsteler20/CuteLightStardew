

import json
from os import path, system

target = "net6.0"

system(f"mkdir -p bin/Debug/{target}/")
system(f"mkdir -p bin/Release/{target}/")

js = json.load(open("obj/project.assets.json", 'r'))

pkgs_path = js["project"]["restore"]["packagesPath"]

for lib, value in js["libraries"].items():
    if lib.startswith("CuteLight.Sdk"):
        lib_path = value["path"]

        for f in value["files"]:
            end_seg = f.split("/")[-1]
            if end_seg.endswith(".so") or end_seg.endswith(".dll"):
                print(path.join(pkgs_path,lib_path, f))

                if not path.exists(f"bin/Debug/{target}/{end_seg}"):
                    system(f"cp {path.join(pkgs_path,lib_path, f)} bin/Debug/{target}/")
                if not path.exists(f"bin/Release/{target}/{end_seg}"):
                    system(f"cp {path.join(pkgs_path,lib_path, f)} bin/Release/{target}/")