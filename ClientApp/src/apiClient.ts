// event should be of type EMEvent (or undefined, I guess? for creating an event?)
export async function uploadFiles(files: File[], event: any) {
  const data = new FormData();
  files.forEach((file, i) => {
    const id = event.files.length > 0 ? event.files[event.files.length - 1].id + 1 : i;
    data.append(`${id}`, file);
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

// also ignore this
export async function getBlobs(id: number) {
  const response = await fetch(`/api/files/search?eventId=${id}`);
  const json = response.json();
  return json;
}