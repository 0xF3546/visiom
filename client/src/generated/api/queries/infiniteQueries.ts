// generated with @7nohe/openapi-react-query-codegen@1.6.2 

import { InfiniteData, UseInfiniteQueryOptions, useInfiniteQuery } from "@tanstack/react-query";
import { SearchService } from "../requests/services.gen";
import * as Common from "./common";
export const useSearchServiceGetApiSearchInfinite = <TData = InfiniteData<Common.SearchServiceGetApiSearchDefaultResponse>, TError = unknown, TQueryKey extends Array<unknown> = unknown[]>({ pageSize, q }: {
  pageSize?: number;
  q?: string;
} = {}, queryKey?: TQueryKey, options?: Omit<UseInfiniteQueryOptions<TData, TError>, "queryKey" | "queryFn">) => useInfiniteQuery({
  queryKey: Common.UseSearchServiceGetApiSearchKeyFn({ pageSize, q }, queryKey), queryFn: ({ pageParam }) => SearchService.getApiSearch({ page: pageParam as number, pageSize, q }) as TData, initialPageParam: "1", getNextPageParam: response => (response as {
    nextPage: string;
  }).nextPage, ...options
});
export const useSearchServiceGetApiSearchImagesInfinite = <TData = InfiniteData<Common.SearchServiceGetApiSearchImagesDefaultResponse>, TError = unknown, TQueryKey extends Array<unknown> = unknown[]>({ pageSize, q }: {
  pageSize?: number;
  q?: string;
} = {}, queryKey?: TQueryKey, options?: Omit<UseInfiniteQueryOptions<TData, TError>, "queryKey" | "queryFn">) => useInfiniteQuery({
  queryKey: Common.UseSearchServiceGetApiSearchImagesKeyFn({ pageSize, q }, queryKey), queryFn: ({ pageParam }) => SearchService.getApiSearchImages({ page: pageParam as number, pageSize, q }) as TData, initialPageParam: "1", getNextPageParam: response => (response as {
    nextPage: string;
  }).nextPage, ...options
});
