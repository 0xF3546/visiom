// generated with @7nohe/openapi-react-query-codegen@1.6.2 

import { SearchService } from "../requests/services.gen";
import * as Common from "./common";
export const useSearchServiceGetApiSearchInfinite = <TData = InfiniteData<Common.SearchServiceGetApiSearchDefaultResponse>, TError = unknown, TQueryKey extends Array<unknown> = unknown[]>({ nextPageToken, pageSize, q }: {
  nextPageToken?: string;
  pageSize?: number;
  q?: string;
} = {}, queryKey?: TQueryKey, options?: Omit<UseInfiniteQueryOptions<TData, TError>, "queryKey" | "queryFn">) => useInfiniteQuery({
  queryKey: Common.UseSearchServiceGetApiSearchKeyFn({ nextPageToken, pageSize, q }, queryKey), queryFn: ({ pageParam }) => SearchService.getApiSearch({ nextPageToken, page: pageParam as number, pageSize, q }) as TData, initialPageParam: "1", getNextPageParam: response => (response as {
    nextPage: string;
  }).nextPage, ...options
});
