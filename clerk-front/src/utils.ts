
export function parseUTCTimeStamp(timestamp: string): Date {
    const src = timestamp + 'Z';
    return new Date(src)
}

