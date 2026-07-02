// generated with @7nohe/openapi-react-query-codegen@1.6.2 

import { type QueryClient } from "@tanstack/react-query";
import { SearchService } from "../requests/services.gen";
import * as Common from "./common";
export const ensureUseSearchServiceGetApiSearchData = (queryClient: QueryClient, { page, pageSize, q }: {
  page?: number;
  pageSize?: number;
  q?: string;
} = {}) => queryClient.ensureQueryData({ queryKey: Common.UseSearchServiceGetApiSearchKeyFn({ page, pageSize, q }), queryFn: () => SearchService.getApiSearch({ page, pageSize, q }) });
export const ensureUseSearchServiceGetApiSearchImagesData = (queryClient: QueryClient, { page, pageSize, q }: {
  page?: number;
  pageSize?: number;
  q?: string;
} = {}) => queryClient.ensureQueryData({ queryKey: Common.UseSearchServiceGetApiSearchImagesKeyFn({ page, pageSize, q }), queryFn: () => SearchService.getApiSearchImages({ page, pageSize, q }) });
