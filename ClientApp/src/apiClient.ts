export async function uploadFiles(files: FormData) {
    const response = await fetch('/api/files', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(files)
    });
    const json = await response.json();
    return json;
} 

export async function deleteFile(
  file: any, 
  id: number) {
    const response = await fetch(`/api/files/search?eventId=${id}&fileName=${file.name}`, {
      method: 'DELETE',
      headers: {'Content-Type': 'application/json'},
      body: JSON.stringify({ file, id })
    });
    const json = await response.json();
    return json;
}

export async function createEvent(id: number) {
  const response = await fetch(`/api/events/${id}`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' }
  });
  const json = await response.json();
  return {id: id, files: []};
}