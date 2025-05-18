export type LogEntry = {
    event: string;
    properties: Record<string, any>;
};

export function cleanLogEntry(entry: LogEntry): LogEntry {
    function cleanObject(obj: any): any {
        if (Array.isArray(obj)) {
            return obj.filter(el => el.length !== 0).map(cleanObject);
        } else if (typeof obj === 'object' && obj !== null) {
            return Object.fromEntries(Object.entries(obj).map(([k, v]) => [k, cleanObject(v)]).filter(([, v]) => v !== null));
        }
        return obj;
    }

    return {
        event: entry.event,
        properties: cleanObject(entry.properties)
    };
}

export type Recommendation = {
    field: string;
    justification: string;
    type: string;
};