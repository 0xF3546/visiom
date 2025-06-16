// generated with @7nohe/openapi-react-query-codegen@1.6.2 

import { UseQueryOptions, useQuery } from "@tanstack/react-query";
import { SearchService } from "../requests/services.gen";
import * as Common from "./common";
export const useSearchServiceGetApiSearch = <TData = Common.SearchServiceGetApiSearchDefaultResponse, TError = unknown, TQueryKey extends Array<unknown> = unknown[]>({ nextPageToken, page, pageSize, q }: {
  nextPageToken?: string;
  page?: number;
  pageSize?: number;
  q?: string;
} = {}, queryKey?: TQueryKey, options?: Omit<UseQueryOptions<TData, TError>, "queryKey" | "queryFn">) => useQuery<TData, TError>({ queryKey: Common.UseSearchServiceGetApiSearchKeyFn({ nextPageToken, page, pageSize, q }, queryKey), queryFn: () => SearchService.getApiSearch({ nextPageToken, page, pageSize, q }) as TData, ...options });
