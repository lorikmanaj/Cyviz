export const newIdempotencyKey = () =>
    crypto.randomUUID(); // built-in browser UUID generator
