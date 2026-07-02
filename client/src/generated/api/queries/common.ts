// generated with @7nohe/openapi-react-query-codegen@1.6.2 

import { UseQueryResult } from "@tanstack/react-query";
import { SearchService } from "../requests/services.gen";
export type SearchServiceGetApiSearchDefaultResponse = Awaited<ReturnType<typeof SearchService.getApiSearch>>;
export type SearchServiceGetApiSearchQueryResult<TData = SearchServiceGetApiSearchDefaultResponse, TError = unknown> = UseQueryResult<TData, TError>;
export const useSearchServiceGetApiSearchKey = "SearchServiceGetApiSearch";
export const UseSearchServiceGetApiSearchKeyFn = ({ page, pageSize, q }: {
  page?: number;
  pageSize?: number;
  q?: string;
} = {}, queryKey?: Array<unknown>) => [useSearchServiceGetApiSearchKey, ...(queryKey ?? [{ page, pageSize, q }])];
export type SearchServiceGetApiSearchImagesDefaultResponse = Awaited<ReturnType<typeof SearchService.getApiSearchImages>>;
export type SearchServiceGetApiSearchImagesQueryResult<TData = SearchServiceGetApiSearchImagesDefaultResponse, TError = unknown> = UseQueryResult<TData, TError>;
export const useSearchServiceGetApiSearchImagesKey = "SearchServiceGetApiSearchImages";
export const UseSearchServiceGetApiSearchImagesKeyFn = ({ page, pageSize, q }: {
  page?: number;
  pageSize?: number;
  q?: string;
} = {}, queryKey?: Array<unknown>) => [useSearchServiceGetApiSearchImagesKey, ...(queryKey ?? [{ page, pageSize, q }])];
