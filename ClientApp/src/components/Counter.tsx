import React, { useState, FormEvent } from 'react';
import { uploadFiles, deleteFile, downloadFile, getBlobs } from '../apiClient';

// name needs to be unique within a given event as it is right now -- azure uploads, downloads, and deletes based on name as well
interface EventFile {
    name: string;
    size: number;
    type: string;
    id: number;
}


export const Counter: React.FC = () => {

  // don't need any of this -- anywhere currentCount is used should be event.id
  const [currentCount, setCurrentCount] = useState(0);
  const incrementCount = async () => {
    const newCount = currentCount + 1;
    const fileList = await getBlobs(newCount);
    setCurrentCount(newCount);
    setSupportingFiles(fileList);
  }
  const decrementCount = async () => {
    const newCount = currentCount === 0 ? 0 : currentCount - 1;
    const fileList = await getBlobs(newCount);
    setCurrentCount(newCount);
    setSupportingFiles(fileList);
  }



  // this is for what happens within the form
  const [selectedFiles, setSelectedFiles] = useState<File[]>([]);
  const onExcludeFiles = (deletedFile: File) => {
    const formerFiles = [...selectedFiles];
    setSelectedFiles(formerFiles.filter(file => file.name !== deletedFile.name))
  }
  const onSelectFiles = (e: FileList | null) => {
    const formerFiles = [...selectedFiles];
    const newFiles = e && Array.from(e);
    newFiles && setSelectedFiles([...formerFiles, ...newFiles]);
  }


  // this may be unnecessary as well, depending on implementation -- might just be event.files, and might start out as event.files
  const [supportingFiles, setSupportingFiles] = useState<EventFile[]>([]);


  // this simulates creating a new event in event app -- event.id does not exist yet (until createEvent)
  const createNewEvent = async (e: FormEvent) => {
    e.preventDefault();
    // now event.id exists -- and so does container
    await onUploadFiles(selectedFiles, {files: [], id: currentCount});
  }
  // this simulates editing an event -- event.id exists and so does container
  const editEvent = async (e: FormEvent) => {
    e.preventDefault();
    // second parameter is event
    await onUploadFiles(selectedFiles, {id: currentCount, files: supportingFiles});
  }

  const onUploadFiles = async (newFiles: File[], newEvent: {id: number, files: EventFile[]}) => {
    const formerFiles = [...supportingFiles];
    // second parameter is event 
    const finalFiles = await uploadFiles(newFiles, newEvent);
    // this might be getEvent? or something?
    setSupportingFiles([...formerFiles, ...finalFiles]);
  }
  const onDeleteFiles = (deletedFile: EventFile) => {
    const formerFiles = [...supportingFiles];
    // same as above wrt setSupportingFiles
    deleteFile(deletedFile, currentCount).then(() => setSupportingFiles(formerFiles.filter(file => file.name !== deletedFile.name)));
  }


    return (
      <div>
        <h1>Counter</h1>

        <p>This is a simple example of a React component.</p>

        <p aria-live="polite">Event Id: <strong>{currentCount}</strong></p>

        <button className="btn btn-primary" onClick={incrementCount}>Increment</button>
        <button className="btn btn-primary" onClick={decrementCount}>Decrement</button>
      
        <table>
            <caption>Supporting Files</caption>
            <thead>
              <tr>
                <th>Name</th>
                <th>Size</th>
                <th>&nbsp;</th>
              </tr>
            </thead>
            <tbody>
              {supportingFiles.map(result => {
                return (
                  <tr key={result.name}>
                    <td><a 
                      href={downloadFile(result.name, currentCount)}
                      download={result.name} >{result.name}</a></td>
                    <td>{result.size}</td>
                    <td className="text-right">
                      <button
                        type="button"
                        className="button button--danger button--outline"
                        onClick={(e) => onDeleteFiles(supportingFiles.find(x => x.name === result.name)!)}
                      >
                        Delete file
                      </button>
                    </td>
                  </tr>
                );
              })}
            </tbody>
          </table>


          {/* These forms simulate Create Event and Edit Event forms */}
          <form id="file-upload" onSubmit={createNewEvent}>
            <h1>Create Event</h1>
            <FileList selectedFiles={selectedFiles} onExcludeFiles={onExcludeFiles} />
            <input type="file" multiple onChange={(e) => onSelectFiles(e.currentTarget.files)} />
            <button type="submit">Submit Create Event Form</button>
          </form>

          <form id="file-upload-edit" onSubmit={editEvent}>
            <h1>Edit Event</h1>
            <FileList selectedFiles={selectedFiles} onExcludeFiles={onExcludeFiles} />
            <input type="file" multiple onChange={(e) => onSelectFiles(e.currentTarget.files)} />
            <button type="submit">Submit Edit Event Form</button>
          </form>
      </div>
    );
}


const FileList: React.FC<{selectedFiles: File[]; onExcludeFiles: (f: File) => void;}> = ({selectedFiles, onExcludeFiles}) => {
  
  return (
    <table>
            <caption>Selected Files</caption>
            <thead>
              <tr>
                <th>Name</th>
                <th>Size</th>
                <th>&nbsp;</th>
              </tr>
            </thead>
            <tbody>
              {selectedFiles.map(result => {
                return (
                  <tr key={result.name}>
                    <td>{result.name}</td>
                    <td>{result.size}</td>
                    <td className="text-right">
                      <button
                        type="button"
                        className="button button--danger button--outline"
                        onClick={() => onExcludeFiles(selectedFiles.find(x => x.name === result.name)!)}
                      >
                        Remove file
                      </button>
                    </td>
                  </tr>
                );
              })}
            </tbody>
          </table>
  )
}