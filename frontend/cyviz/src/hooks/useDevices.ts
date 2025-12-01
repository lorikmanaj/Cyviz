import { useInfiniteQuery, type InfiniteData } from "@tanstack/react-query";
import { getDevices } from "../api/devices";
import type { DeviceListDto } from "../types/device";
import type { KeysetPageResult } from "../types/pagination/keysetPageResult";

export type Page = KeysetPageResult<DeviceListDto>;

export const useDevices = () =>
    useInfiniteQuery<Page, Error, InfiniteData<Page>, ["devices"], string | undefined>({
        queryKey: ["devices"],
        queryFn: ({ pageParam }) => getDevices(pageParam),
        getNextPageParam: (lastPage) =>
            lastPage.nextCursor ?? undefined,
        initialPageParam: undefined,
    });
