// generated with @7nohe/openapi-react-query-codegen@1.6.2 

import { UseQueryOptions, useQuery } from "@tanstack/react-query";
import { SearchService } from "../requests/services.gen";
import * as Common from "./common";
export const useSearchServiceGetApiSearch = <TData = Common.SearchServiceGetApiSearchDefaultResponse, TError = unknown, TQueryKey extends Array<unknown> = unknown[]>({ page, pageSize, q }: {
  page?: number;
  pageSize?: number;
  q?: string;
} = {}, queryKey?: TQueryKey, options?: Omit<UseQueryOptions<TData, TError>, "queryKey" | "queryFn">) => useQuery<TData, TError>({ queryKey: Common.UseSearchServiceGetApiSearchKeyFn({ page, pageSize, q }, queryKey), queryFn: () => SearchService.getApiSearch({ page, pageSize, q }) as TData, ...options });
export const useSearchServiceGetApiSearchImages = <TData = Common.SearchServiceGetApiSearchImagesDefaultResponse, TError = unknown, TQueryKey extends Array<unknown> = unknown[]>({ page, pageSize, q }: {
  page?: number;
  pageSize?: number;
  q?: string;
} = {}, queryKey?: TQueryKey, options?: Omit<UseQueryOptions<TData, TError>, "queryKey" | "queryFn">) => useQuery<TData, TError>({ queryKey: Common.UseSearchServiceGetApiSearchImagesKeyFn({ page, pageSize, q }, queryKey), queryFn: () => SearchService.getApiSearchImages({ page, pageSize, q }) as TData, ...options });
