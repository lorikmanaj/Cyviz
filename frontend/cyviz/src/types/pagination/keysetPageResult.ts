export interface KeysetPageResult<T> {
    items: T[];
    nextCursor?: string | null;
}
