export async function uploadFiles(files: FormData, eventId: number) {
  files.forEach(x => console.log(x));
    const response = await fetch(`/api/files/upload?eventId=${eventId}`, {
      method: 'POST',
      body: files
    });
    const json = await response.json();
    return json;
} 

export async function deleteFile(
  file: any, 
  id: number) {
    await fetch(`/api/files/search?eventId=${id}&fileName=${file.name}`, {
      method: 'DELETE'
    });
}

export async function downloadFile(fileName: string, eventId: number) {
  const response = await fetch(`/api/files/search?eventId=${eventId}&fileName=${fileName}`);
}

export async function createEvent(id: number) {
  const response = await fetch(`/api/events/${id}`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' }
  });
  const json = await response.json();
  return {id: id, files: []};
}