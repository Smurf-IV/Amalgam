using System;
using System.IO;

namespace AmalgamClientTray.FTP
{
   /// <summary>
   /// Stolen from http://stackoverflow.com/questions/3240968/should-dispose-or-finalize-be-used-to-delete-temporary-files
   /// </summary>
   /// <example>
   /// using (TemporaryFile temporaryFile = new TemporaryFile(true)) 
   /// { 
   ///    temporaryFile.Keep = true; 
   /// }
   /// </example>
   public class TemporaryFile : IDisposable
   {
      private bool isDisposed;

      /// <summary>
      /// If you decide, after constructing a TemporaryFile, that you want to prevent it from being deleted, 
      /// simply set the TemporaryFile.Keep property to true:
      /// </summary>
      public bool Keep { get; set; }

      /// <summary>
      /// Accessor for the generated filePathName
      /// </summary>
      public string FilePath { get; private set; }

      /// <summary>
      /// Default constructor
      /// </summary>
      public TemporaryFile()
         : this(false)
      {
      }

      /// <summary>
      /// Constructor for the in memory cache version
      /// </summary>
      /// <param name="shortLived"></param>
      public TemporaryFile(bool shortLived)
      {
         FilePath = CreateTemporaryFile(shortLived);
      }

      /// <summary>
      /// Use C# destructor syntax for finalization code.
      /// This destructor will run only if the Dispose method does not get called.
      /// It gives your base class the opportunity to finalize.
      /// Do not provide destructors in types derived from this class.      
      /// </summary>
      ~TemporaryFile()
      {
         Dispose(false);
      }

      public void Dispose()
      {
         Dispose(true);
         // This object will be cleaned up by the Dispose method.
         // Therefore, you should call GC.SupressFinalize to take this object 
         // off the finalization queue and prevent finalization code for this 
         // object from executing a second time.
         GC.SuppressFinalize(this);
      }

      /// <summary>
      /// Dispose(bool disposing) executes in two distinct scenarios.
      /// If disposing equals true, the method has been called directly
      /// or indirectly by a user's code. Managed and unmanaged resources
      /// can be disposed.
      /// If disposing equals false, the method has been called by the
      /// runtime from inside the finalizer and you should not reference
      /// other objects. Only unmanaged resources can be disposed.
      /// </summary>
      /// <param name="disposing"></param>
      protected virtual void Dispose(bool disposing)
      {
         if (!isDisposed)
         {
            isDisposed = true;

            if (!this.Keep)
            {
               TryDelete();
            }
         }
      }

      private void TryDelete()
      {
         try
         {
            File.Delete(FilePath);
         }
         catch (IOException)
         {
         }
         catch (UnauthorizedAccessException)
         {
         }
      }

      /// <summary>
      /// Use this in an attempt to cache in memory.
      /// </summary>
      /// <param name="shortLived"></param>
      /// <returns></returns>
      public static string CreateTemporaryFile(bool shortLived)
      {
         string temporaryFile = Path.GetTempFileName();
         
         if (shortLived)
         {
            // Set the temporary attribute
            File.SetAttributes(temporaryFile, File.GetAttributes(temporaryFile) | FileAttributes.Temporary);
         }

         return temporaryFile;
      }
   }
}
