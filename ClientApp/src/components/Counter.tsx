import React, { useState } from 'react';
import { uploadFiles, deleteFile, createEvent } from '../apiClient';


interface EventFile {
    id: number;
    name: string;
    path: ArrayBuffer | string | null;
    size: number;
    type: string;
  
}


export const Counter: React.FC = () => {

  const [currentCount, setCurrentCount] = useState(0);

  const incrementCounter = () => {
    setCurrentCount(currentCount + 1);
  }

  const [event, setEvent] = useState<{files: EventFile[], id: number}>({files: [], id: 1});
  const accountName = "quickstarttest"
  const [supportingFiles, setSupportingFiles] = useState<EventFile[]>([...event.files]);
  const createNewEvent = async () => {
    const newEvent = await createEvent(currentCount);
    setEvent(newEvent);
  }
  const onSelectFiles = async (e: FileList | null) => {
    const formerFiles = [...supportingFiles];
    const newFiles = e && Array.from(e); 
    const data = new FormData();
    newFiles && newFiles.forEach((file, i) => { 
      const id = formerFiles.length > 0 ? formerFiles[formerFiles.length - 1].id + i + 1: i; 
      data.append(`file-${id}`, file, file.name);
    });
    data.append("eventId", event.id.toString());
    const finalFiles = data ? await uploadFiles(data) : undefined;
    setSupportingFiles([...formerFiles, ...finalFiles]);
  }

  const onDeleteFiles = (deletedFile: EventFile) => {
    const formerFiles = [...supportingFiles];
    setSupportingFiles(formerFiles.filter(file => file.id !== deletedFile.id));
    if (deletedFile.path) {
      deleteFile(deletedFile, event.id);
    }
  }
  const submitFileChanges = async () => {
    setEvent({id: event.id, files: supportingFiles});
  }

  const excludeDuplicates = (mayHaveDuplicates: any[]) => {
    const response: any[] = [];
    mayHaveDuplicates.forEach((i: { id: any; }) => {
      const isFound = response.some(j => {
        return i.id === j.id;
      });
      if (!isFound) response.push(i);
    });
    return response;
  };

  const excludeFile = (e: HTMLButtonElement) => {
    const buttonId = parseInt(e.getAttribute('data-key')!);
    const file = supportingFiles.find((i: { id: number; }) => i.id === buttonId)
    if (file) {
      deleteFile(file, event.id);
      return supportingFiles.filter((i: { id: number; }) => i.id !== buttonId);
    } 
  };


    return (
      <div>
        <h1>Counter</h1>

        <p>This is a simple example of a React component.</p>

        <p aria-live="polite">Current count: <strong>{currentCount}</strong></p>

        <button className="btn btn-primary" onClick={incrementCounter}>Increment</button>
      
        <table>
            <caption>Supporting Files</caption>
            <thead>
              <tr>
                <th>Name</th>
                <th>Path</th>
                <th>Size</th>
                <th>&nbsp;</th>
              </tr>
            </thead>
            <tbody>
              {excludeDuplicates(supportingFiles).map(result => {
                return (
                  <tr key={result.id}>
                    <td>{result.name}</td>
                    <td>{result.path}</td>
                    <td>{result.size}</td>
                    <td className="text-right">
                      <button
                        type="button"
                        className="button button--danger button--outline"
                        data-key={result.id}
                        onClick={(e: { currentTarget: HTMLButtonElement; }) => excludeFile(e.currentTarget)}
                      >
                        Remove market
                      </button>
                    </td>
                  </tr>
                );
              })}
            </tbody>
          </table>
          <button onClick={createNewEvent}>Create Container</button>
          <form id="file-upload" onSubmit={submitFileChanges}>
            <input type="file" multiple onChange={(e) => onSelectFiles(e.currentTarget.files)} />
            <button type="submit">Submit</button>
          </form>
      </div>
    );
}
