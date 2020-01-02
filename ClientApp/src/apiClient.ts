// event should be of type EMEvent (or undefined, I guess?)
export async function uploadFiles(files: File[], event: any) {
  const data = new FormData();
  files.forEach(file => {
    const id = event.files.length > 0 ? event.files[event.files.length - 1].id + files.indexOf(file) + 1: files.indexOf(file); 
    data.append('id', file)
  });
  const response = await fetch(`/api/files/upload?eventId=${event.id}`, {
    method: 'POST',
    body: data
  });
    const json = await response.json();
    // this is a list of type EventFile, the newly uploaded files
    return json;
} 

export async function deleteFile(
  file: any, 
  id: number) {
    await fetch(`/api/files/delete?eventId=${id}&fileName=${file.name}`, {
      method: 'DELETE'
    });
}

export function downloadFile(fileName: string, eventId: number) {
  return `/api/files/download?eventId=${eventId}&fileName=${fileName}`
}

export async function createEvent(id: number) {
  const response = await fetch(`/api/events/${id}`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' }
  });
  const json = await response.json();
  return {id: id, files: []};
}