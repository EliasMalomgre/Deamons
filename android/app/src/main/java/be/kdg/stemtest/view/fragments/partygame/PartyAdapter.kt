package be.kdg.stemtest.view.fragments.partygame

import android.content.Context
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.navigation.findNavController
import androidx.recyclerview.widget.RecyclerView
import be.kdg.stemtest.R
import be.kdg.stemtest.model.entity.Party
import be.kdg.stemtest.viewmodel.PartyViewModel
import com.squareup.picasso.Picasso
import kotlinx.android.synthetic.main.partij_optie_fragment.view.*


class PartyAdapter (
 val context: Context?,
 val viewModel: PartyViewModel
) : RecyclerView.Adapter<PartyAdapter.PartyViewHolder>() {

   private var parties: List<Party> = listOf()
        set(party) {
            field = party
            notifyDataSetChanged()
        }


    class PartyViewHolder(view: View) : RecyclerView.ViewHolder(view) {
        val name= view.txtName
        val partyLogo=view.partyLogo
    }

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): PartyViewHolder {
        val partyView = LayoutInflater.from(parent.context).inflate(R.layout.partij_optie_fragment, parent, false)
        return PartyViewHolder(
            partyView
        )
    }

    override fun getItemCount() : Int {
       return  parties.size
    }

    override fun onBindViewHolder(holder: PartyViewHolder, position: Int) {
        holder.name.text = parties?.get(position)!!.name
        Picasso.get().load(parties[position].logo).into(holder.partyLogo)



        holder.itemView.setOnClickListener{
                v ->
            viewModel.chooseParty(parties[position])
            val navController = v.findNavController()
            navController.navigate(R.id.wachtscherm)
        }
    }

    fun setData(parties:List<Party>){
        this.parties = parties
        notifyDataSetChanged()
    }

    override fun getItemViewType(position: Int): Int {
            return R.layout.partij_optie_fragment
    }
}