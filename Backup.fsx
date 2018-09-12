//Process.Start("/bin/bash", "-c \"mysqldump -uroot -p1234 tender2 | /bin/gzip -c > /home/alex/backup/sql/111.sql.gz\"").WaitForExit()
module Backup

open System
open System.IO
open System.Diagnostics
open System.Linq
open System.IO

let date = DateTime.Now.ToString("yyyy-MM-dd_HH-mm")
let user = "root"
let password = "Dft56Point"
let bd_name = "tender"
let bd_name_users = "uto_users"
let bk_dir_f = "/mnt/hdd00/backup"
let bk_dir_db = "/mnt/hdd00/backup/sql"
let inf_dir = "/srv"
let list_dir_db = [ bk_dir_db ]
let list_dir_file = [ bk_dir_f ]
let dir_to_bk = "tenders.enter-it.ru"
let exec_command_f = sprintf "-czvf %s/www_%s.tar.gz -C %s %s" bk_dir_f date inf_dir dir_to_bk

Process.Start("/bin/tar", exec_command_f).WaitForExit()

let exec_command_dump_bd =
    sprintf "-c \"mysqldump -u%s -p%s %s | /bin/gzip -c > %s/mysql_%s_%s.sql.gz\"" user password bd_name bk_dir_db 
        bd_name date

Process.Start("/bin/bash", exec_command_dump_bd).WaitForExit()

let exec_command_dump_bd_users =
    sprintf "-c \"mysqldump -u%s -p%s %s | /bin/gzip -c > %s/mysql_%s_%s.sql.gz\"" user password bd_name_users bk_dir_db 
        bd_name_users date

Process.Start("/bin/bash", exec_command_dump_bd_users).WaitForExit()

let delfile tm dir =
    try 
        System.IO.Directory.GetFiles(dir).Select(fun f -> new FileInfo(f))
              .Where(fun f -> f.LastWriteTime < DateTime.Now.AddDays(tm)).ToList().ForEach(fun f -> f.Delete())
    with _ -> printfn "error"

list_dir_db |> List.iter (delfile -7.)
list_dir_file |> List.iter (delfile -15.)
