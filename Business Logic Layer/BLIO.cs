﻿using ClipboardWatcher;
using Data_Access_Layer;
using System;
using System.IO;

namespace Business_Logic_Layer
{
    public class BLIO
    {
        /// <summary>
        ///  Writes an error to the errorlog.txt
        /// </summary>
        /// <param name="ex">The occured exception</param>
        /// <param name="message">A short message i.e "Error while loading reminders"</param>
        /// <param name="showErrorPopup">true to pop up an additional windows form to show the user that an error has occured</param>
        public static void WriteError(Exception ex, string message)
        {
            //The bunifu framework makes a better looking ui, but it also throws annoying null reference exceptions when disposing an form/usercontrol
            //that has an bunifu control in it(like a button), while there shouldn't be an exception.
            if ((ex is System.Runtime.InteropServices.ExternalException) && ex.Source == "System.Drawing" && ex.Message.Contains("GDI+"))
                return;

            using (FileStream fs = new FileStream(IOVariables.errorLog, FileMode.Append))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.WriteLine("[" + DateTime.Now + "] - " + message + "\r\n" + ex.ToString() + "\r\n\r\n");
            }
        }
        public static void CreateDatabaseIfNotExist()
        {
            if (!System.IO.File.Exists(IOVariables.databaseFile))
                DLDatabase.CreateDatabase();
            else
            {
                //great! the .db file exists. Now lets check if the user's .db file is up-to-date. let's see if the reminder table has all the required columns.
                if (DLDatabase.HasAllTables())
                {
                    if (!DLDatabase.HasAllColumns())
                        DLDatabase.InsertNewColumns(); //not up to date. insert !
                }
                else
                {
                    DLDatabase.InsertMissingTables();
                    //re-run the method, since the .db file **should** now have all the tables.
                    CreateDatabaseIfNotExist();
                }

            }


        }

        public static void DeleteEmptyDirectories(string path)
        {
            foreach (string directory in Directory.GetDirectories(path,"*",SearchOption.AllDirectories))
            {
                if (Directory.GetFileSystemEntries(directory).Length == 0)
                    Directory.Delete(directory, false);                
            }
        }
    }
}
