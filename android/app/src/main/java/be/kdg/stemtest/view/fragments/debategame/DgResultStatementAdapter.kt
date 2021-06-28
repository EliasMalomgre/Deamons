package be.kdg.stemtest.view.fragments.debategame

import android.content.Context
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.core.os.bundleOf
import androidx.navigation.findNavController
import androidx.recyclerview.widget.RecyclerView
import be.kdg.stemtest.R
import be.kdg.stemtest.model.entity.*
import com.squareup.picasso.Picasso
import kotlinx.android.synthetic.main.dg_result_statement_fragment.view.*

class DgResultStatementAdapter(val context: Context?) :
    RecyclerView.Adapter<DgResultStatementAdapter.DgResultViewHolder>() {


    private var results: List<DebateGameResult> = listOf()
        set(result) {
            field = result
            notifyDataSetChanged()
        }

    private var parties: List<Party> = listOf()
        set(result) {
            field = result
            notifyDataSetChanged()
        }


    class DgResultViewHolder(view: View) : RecyclerView.ViewHolder(view) {
        val partyImage = view.resultPartyLogo
        val partyName = view.txtYourAnswer
        val percentage = view.txtPercentage
        val moreInfo = view.btnInfo

    }


    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): DgResultViewHolder {
        val dgResultStatementView = LayoutInflater.from(parent.context)
            .inflate(R.layout.dg_result_statement_fragment, parent, false)
        return DgResultViewHolder(
            dgResultStatementView
        )
    }

    override fun getItemCount(): Int {
        if (results.isNullOrEmpty()) {
            return 0
        } else
            return results!!.size
    }


    override fun onBindViewHolder(holder: DgResultViewHolder, position: Int) {
        val result = results[position]
        holder.partyName.text = result.partyName
        holder.percentage.text = result.percentage
        Picasso.get().load(parties.find { p -> p.name == result.partyName }?.logo)
            .into(holder.partyImage)

        holder.moreInfo.setOnClickListener{
            v->
            val bundle = bundleOf("partyName" to result.partyName,"gameType" to 2)
            val navController = v.findNavController()
            navController.navigate(R.id.pgdg_more_info,bundle)
        }


    }

    fun setResultData(results: List<DebateGameResult>) {
        this.results = results
        notifyDataSetChanged()
    }


    fun setPartytData(results: List<Party>) {
        this.parties = results
        notifyDataSetChanged()
    }


}