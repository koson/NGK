/**
 *
 * NAME:    cmdlnprs.c
 *
 * DESCRIPTION: Simple string tokenizer and command line parser
 *              implementation
 *
 *
 * AUTHOR:  Alexander Lokotkov
 *
 *
 *******************************************************
 *      Copyright (C) 2000-2010 Fastwel Group.
 *      All rights reserved.
 ********************************************************/

#if (!defined WINDOWS_CE_ISR_CODE) && (!defined KERNEL_DRIVER_CODE)

#include <eal/common/cmdlnprs.h>

static bool is_decimal_digit(F_CHAR in_char)
{
  return in_char >= EAL_T('0') && in_char <= EAL_T('9');
}

static bool is_digit(F_CHAR in_char, bool hex)
{
  return  (in_char >= EAL_T('0') && in_char <= EAL_T('9')) || 
          (hex && ( (in_char >= EAL_T('A') && in_char <= EAL_T('F')) || (in_char >= EAL_T('a') && in_char <= EAL_T('f')) ));
}

static bool is_c_dec(const F_STR chars, size_t len)
{
  size_t ch;
  for( ch = 0; ch < len && is_decimal_digit(chars[ch]); ++ch);

  return ch == len;
}

static bool is_c_hex(const F_STR chars, size_t len)
{
  if( len > 2 && chars[0] == EAL_T('0') && ( chars[1] == EAL_T('x') || chars[1] == EAL_T('X') ) )
  {
    size_t ch;
    for( ch = 2; ch < len && is_digit(chars[ch], true); ++ch);
    return ch == len;
  }
  return false;
}

#define CH_QUOTE EAL_T('\"')

bool F_LineTokenizer_next(PF_LINE_TOKEN pToken, const F_STR delims)
{
  if(pToken && pToken->line)
  {
    // default case, if no delimeters passed
    F_STR  no_delim = EAL_T(" \t");
    // use the default delimeter ' ' or passed with the delims
    F_STR delim_array = delims && el_strlen(delims) ? delims : no_delim;
    // total number of delimeters
    size_t delims_count = el_strlen(delim_array);
    // search position
    size_t pos = pToken->pos == LT_INVALID_POS ? 0 : pToken->pos + pToken->len;
    // EAL_T('\"') count
    size_t quote_count = 0;

    // reset the token
    //F_LineTok_reset(pToken);
    pToken->left_delim = LT_EMPTY_DELIMETER;
    pToken->pos = LT_INVALID_POS;
    pToken->len = 0;

    for(; pToken->line[pos] != EAL_T('\0'); ++pos)
    {
      size_t delim_num;

      if(pToken->line[pos] == CH_QUOTE)
        ++quote_count;

      for(delim_num = 0; delim_num < delims_count; ++delim_num)
      {
        if(pToken->pos == LT_INVALID_POS)
        {
          // token start was not found out yet
          if(pToken->line[pos] != delim_array[delim_num])
          {
            // if it's not the last delimeter, try to test against another delimeter
            if(delim_num != delims_count - 1)
              continue;
            // all delimeters tested,
            // store the start token position, and break to the outer loop
            pToken->pos = pos;
          }
          else
          {
            if(delim_array[delim_num] == CH_QUOTE)
              --quote_count;
            // store delimeter on the left
            pToken->left_delim = delim_array[delim_num];
          }
          break;
        }
        else if(pToken->line[pos] == delim_array[delim_num])
        {
          if(0 == (quote_count % 2))
          {
            // token start has been found out, so we need to search for the next delimeter
            pToken->len = pos - pToken->pos;
            return true;
          }
        }
      }
    }

    if(pToken->pos != LT_INVALID_POS)
    {
      pToken->len = pos - pToken->pos;
      return true;
    }
  }
  return false;
}

size_t F_LineTokenizer_copy(const PF_LINE_TOKEN pToken, F_STR dst_buffer, size_t buffer_size, bool strip_quotes)
{
  size_t chars_copied = 0;
  if(dst_buffer != NULL)
  {
    if(pToken != NULL && pToken->line != NULL && pToken->pos != LT_INVALID_POS && pToken->len != 0)
    {
      size_t cur_pos;// = pToken->pos;
      size_t token_last_pos = pToken->pos + pToken->len;

      for(cur_pos = pToken->pos; cur_pos < token_last_pos && chars_copied < (buffer_size - 1); ++cur_pos)
      {
        if(strip_quotes && pToken->line[cur_pos] == CH_QUOTE)
          continue;

        dst_buffer[chars_copied] = pToken->line[cur_pos];
        ++chars_copied;
      }
    }
    dst_buffer[chars_copied] = EAL_T('\0');
  }
  return chars_copied;
}


bool   F_LineParser_getVariable(PF_LINE_PARSER_VAR pVariable, const PF_LINE_TOKEN pToken, const F_STR value_delims)
{
  if(pVariable && F_LineTok_isValid(pToken))
  {
    F_STR var_line = pToken->line + pToken->pos;
    F_LINE_TOKEN var_tok;

    // prepare the variable line tokenizer
    F_LineTok_init(&var_tok, var_line);

    // get the name
    if(F_LineTokenizer_next(&var_tok, value_delims) && var_tok.len > 0)
    {
      // seems like it's been gotten
      if(F_LineTokenizer_copy(&var_tok, pVariable->name, LN_PARSER_VAR_NAME_LEN, true))
      {
        // the var name has been copied to pVariable, get the value
        if(F_LineTokenizer_next(&var_tok, value_delims) && var_tok.len > 0)
        {
          int value_base =    is_c_dec(&var_tok.line[var_tok.pos], var_tok.len) ? 10 :
                             (is_c_hex(&var_tok.line[var_tok.pos], var_tok.len) ? 16 : 0);
            
          if(value_base != 0)
          {
            // it is a number
            F_STR end;
            unsigned long ulv = el_strtoul(&var_tok.line[var_tok.pos], &end, value_base);

            if(&var_tok.line[var_tok.pos] < end)
            {
              // it's ok, u32 has been read
              pVariable->type = LNP_LONG;
              pVariable->value.val_ulong = ulv;
              return true;
            }
          }
          else
          {
            // it is a string
            if(F_LineTokenizer_copy(&var_tok, pVariable->value.val_string, LN_PARSER_VAR_STR_LEN, true))
            {
              // it's ok, the string has been read
              pVariable->type = LNP_STRING;
              return true;
            }
          }
        }
      }
    }
    // reset the variable, so the user couldn't use it
    F_LineVar_init(pVariable);
  }
  return false;
}

bool   F_LineParser_copyVariable(PF_LINE_PARSER_VAR pDestination, const PF_LINE_PARSER_VAR pSource)
{
  if(pSource && pDestination && !F_LineVar_isEmpty(pSource))
  {
    if( LNP_STRING == F_LineVar_type(pSource) )
    {
      el_strcpy(pDestination->value.val_string, pSource->value.val_string);
    }
    else if( LNP_LONG == F_LineVar_type(pSource) )
    {
      pDestination->value.val_ulong = pSource->value.val_ulong;
    }
    else
    {
      F_LineVar_init(pDestination);
      return false;
    }

    pDestination->type = pSource->type;
    el_strcpy(pDestination->name, pSource->name);
    return true;
  }
  return false;
}

//#ifdef LINE_TOKENIZER_TEST

#include <eal/debug/idbg.h>
/**
 *    Test usage:
 *    F_STR test_string = EAL_T(" asss -t:acdef -k 0x280 -v 1234 --p:sdsds");
 *    F_STR tok_delims = EAL_T("-");
 *    F_STR value_delims = EAL_T(" :");
 *    F_LineTokenizer_test(test_string, tok_delims, value_delims);
 *
 */
void F_LineParser_test(const F_STR test_string, const F_STR tokens_delimeters, const F_STR values_delimeters)
{
  F_LINE_TOKEN tok;
  F_CHAR token_buffer[128];
  F_CHAR delim_buffer[2];

  F_DBG1("Line parser test. Input: %s\n", test_string);
  F_DBG2("Token delimeters: %s Value delimeters: %s\n", tokens_delimeters, values_delimeters);

  F_LineTok_init(&tok, test_string);

  while(F_LineTokenizer_next(&tok, tokens_delimeters))
  {
    F_LineTokenizer_copy(&tok, token_buffer, 128, false);
    delim_buffer[0] = tok.left_delim;
    delim_buffer[1] = EAL_T('\0');
    F_DBG2("Token: %s Delimeter: %s\n", token_buffer, delim_buffer);
  }

#if 1
  F_LineTok_init(&tok, test_string);

  while(F_LineTokenizer_next(&tok, tokens_delimeters))
  {
    if(tok.left_delim == EAL_T('-'))
    {
      F_LINE_PARSER_VAR var;
      if(F_LineParser_getVariable(&var, &tok, values_delimeters))
      {
        if (F_LineVar_type(&var) == LNP_LONG)
        {
          F_DBG3("Number var name: %s value: %lu(%lX)\n", F_LineVar_name(&var), F_LineVar_getLong(&var), F_LineVar_getLong(&var));
        }
        else if (F_LineVar_type(&var) == LNP_STRING)
        {
          F_DBG2("String var name: %s value: %s\n", F_LineVar_name(&var), F_LineVar_getString(&var));
        }
        else
        {
          F_DBG("Unknown variable!\n");
        }
      }
    }
  }
#endif

  F_DBG("Finished!\n");
}
#endif